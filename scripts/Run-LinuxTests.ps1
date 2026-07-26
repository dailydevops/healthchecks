<#
.SYNOPSIS
    Runs the repo's test projects inside a Linux Docker container, bypassing the Windows-only
    "caller-user" gRPC header bug some clients (e.g. RocketMQ.Client) hit when the local Windows
    account name contains non-ASCII characters (e.g. "MartinStühmer").

.DESCRIPTION
    This is a LOCAL DEV CONVENIENCE ONLY. It does not touch, replace, or otherwise affect the CI
    pipeline, which keeps running tests the standard way via the reusable GitHub Actions workflow.

    The script spins up (or reuses) a long-lived Linux container with an ASCII-only username,
    syncs the current working tree into it (including uncommitted changes, excluding bin/obj/.git),
    restores + builds the requested test project for the requested TargetFramework, and runs it
    with TRX + Cobertura coverage output. Results are copied back to ./TestResults-Linux.

.PARAMETER TestProject
    Which test project to run: 'Unit' or 'Integration'. Default: 'Integration'.

.PARAMETER Filter
    Optional TUnit --treenode-filter expression (e.g. "/*/*/*RocketMQ*/*"). Omit to run the whole
    project. Note: TUnit's filter glob doesn't always match parameterized/data-driven tests at the
    expected path depth - if in doubt, omit the filter and let it run everything (fast for Unit,
    slower for Integration).

.PARAMETER TargetFramework
    net8.0, net9.0 or net10.0. Default: net9.0.

.PARAMETER Configuration
    Build configuration. Default: Release.

.PARAMETER Fresh
    Remove any existing container and rebuild it from scratch (re-installs docker CLI, git, rsync,
    and the .NET 10 SDK). Use this if the container's tooling ever needs a reset; otherwise the
    container is reused across runs to skip that setup cost.

.PARAMETER ContainerName
    Name of the persistent helper container. Default: healthchecks-linux-tests.

.EXAMPLE
    ./scripts/Run-LinuxTests.ps1 -TestProject Integration -Filter "/*/*/*RocketMQ*/*"

.EXAMPLE
    ./scripts/Run-LinuxTests.ps1 -TestProject Unit -Fresh
#>
[CmdletBinding()]
param(
    [ValidateSet('Unit', 'Integration')]
    [string]$TestProject = 'Integration',

    [string]$Filter,

    [ValidateSet('net8.0', 'net9.0', 'net10.0')]
    [string]$TargetFramework = 'net10.0',

    [string]$Configuration = 'Release',

    [switch]$Fresh,

    [string]$ContainerName = 'healthchecks-linux-tests'
)

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')

function Test-DockerAvailable {
    $null = docker version 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Docker doesn't seem to be running. Start Docker Desktop / Rancher Desktop and try again."
    }
}

Test-DockerAvailable

if ($Fresh) {
    Write-Host "Removing existing container '$ContainerName' (if any)..." -ForegroundColor Yellow
    docker rm -f $ContainerName 2>$null | Out-Null
}

$existing = docker ps -aq -f "name=^$ContainerName`$"

if (-not $existing) {
    Write-Host "Creating container '$ContainerName'..." -ForegroundColor Cyan
    docker run -d --name $ContainerName `
        -v /var/run/docker.sock:/var/run/docker.sock `
        -v "${repoRoot}:/src:rw" `
        -e USER=ciuser -e USERNAME=ciuser -e HOME=/root -e DOTNET_CLI_HOME=/root `
        mcr.microsoft.com/dotnet/sdk:9.0 sleep infinity | Out-Null

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create container."
    }

    Write-Host "Installing container tooling (docker CLI, git, rsync, sudo, .NET 10 SDK)..." -ForegroundColor Cyan
    # sudo is required because some test assemblies (e.g. DB2) probe for it in an assembly-level
    # hook even when the actual DB2 tests aren't selected by a filter - without it, that hook
    # throws and cascades into every test in the same assembly failing.
    docker exec $ContainerName bash -lc "apt-get update -qq && apt-get install -y -qq docker.io git rsync curl sudo >/dev/null 2>&1"
    docker exec $ContainerName bash -lc "curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh && bash /tmp/dotnet-install.sh --channel 10.0 --install-dir /usr/share/dotnet"
}
else {
    $running = docker ps -q -f "name=^$ContainerName`$"
    if (-not $running) {
        Write-Host "Starting existing container '$ContainerName'..." -ForegroundColor Cyan
        docker start $ContainerName | Out-Null
    }
    else {
        Write-Host "Reusing running container '$ContainerName'..." -ForegroundColor Cyan
    }
}

Write-Host "Syncing working tree into container (including uncommitted changes)..." -ForegroundColor Cyan
docker exec $ContainerName bash -lc "mkdir -p /repo && rsync -a --delete --exclude='bin/' --exclude='obj/' --exclude='.git/' /src/ /repo/"

$projectFolder = if ($TestProject -eq 'Unit') { 'NetEvolve.HealthChecks.Tests.Unit' } else { 'NetEvolve.HealthChecks.Tests.Integration' }
$projectPath = "tests/$projectFolder/$projectFolder.csproj"

Write-Host "Restoring and building $projectFolder ($TargetFramework, $Configuration)..." -ForegroundColor Cyan
docker exec $ContainerName bash -lc "cd /repo && dotnet restore $projectPath && dotnet build $projectPath -f $TargetFramework -c $Configuration"

if ($LASTEXITCODE -ne 0) {
    throw "Build failed. See output above."
}

$exePath = "/repo/tests/$projectFolder/bin/$Configuration/$TargetFramework/$projectFolder"
$filterArg = if ($Filter) { "--treenode-filter `"$Filter`"" } else { '' }
$trxName = "local-run-$TestProject.trx"
$coverageName = "local-run-$TestProject-coverage.cobertura.xml"

Write-Host "Running tests..." -ForegroundColor Cyan
docker exec $ContainerName bash -lc "cd /repo && $exePath $filterArg --report-trx --report-trx-filename $trxName --coverage --coverage-output-format cobertura --coverage-output $coverageName"
$testExitCode = $LASTEXITCODE

$resultsDir = Join-Path $repoRoot "TestResults-Linux"
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
docker cp "${ContainerName}:/repo/tests/$projectFolder/bin/$Configuration/$TargetFramework/TestResults/." $resultsDir 2>$null | Out-Null

# Sync back any new/changed Verify snapshot artifacts (.received.txt for review, and any
# .verified.txt that AutoVerify auto-accepted because no baseline existed yet) so they show up
# in the real working tree for git diff / manual accept, exactly like a local test run would.
Write-Host "Syncing back changed snapshot files..." -ForegroundColor Cyan
docker exec $ContainerName bash -lc "rsync -a /repo/tests/$projectFolder/_snapshots/ /src/tests/$projectFolder/_snapshots/ 2>/dev/null || true"

Write-Host ""
Write-Host "Results copied to: $resultsDir" -ForegroundColor Green
Write-Host "Snapshot changes (if any) synced back into tests/$projectFolder/_snapshots - check 'git status' for .received.txt files to review." -ForegroundColor Green
Write-Host "Container '$ContainerName' left running for reuse. Remove it with: docker rm -f $ContainerName" -ForegroundColor DarkGray
Write-Host "Re-run with -Fresh if the container's tooling ever needs a clean reset." -ForegroundColor DarkGray

exit $testExitCode

<#
    .SYNOPSIS
    Retrieves the GitHub Actions 'testgroup' configuration for each test project
    and ensures all test projects have a valid configuration.

    .DESCRIPTION
    Scans all test projects under 'tests', reads '.testgroup' for CI runner configuration,
    throws an error if missing, and outputs filtered projects as compressed JSON.
#>

# Set Location to Parents parent directory
$currentDirectory = Get-Location
Set-Location (Split-Path (Split-Path $MyInvocation.MyCommand.Definition -Parent) -Parent)

# Get all testgroups from the test projects
$testGroups = Get-ChildItem -Path "tests/NetEvolve.HealthChecks.Tests.Integration" -Recurse -Filter "*.csproj" | ForEach-Object {
  $projectDir = $_.Directory.FullName

  # Get all .testgroup files in the project directory
  $testGroupFile = Join-Path -Path $projectDir -ChildPath ".testgroup"

  if (-Not (Test-Path -Path $testGroupFile)) {
    throw "Missing .testgroup file in test project: $($_.FullName)"
  }

  $testGroupContent = Get-Content -Path $testGroupFile -Raw
  $testGroupContent.Trim()

  # Also validate that each direct subdirectory has a .testgroup file, excluding nested ones and the folder bin, obj, _snapshots and Internals
  Get-ChildItem -Path $projectDir -Directory | Where-Object {
    $_.Name -notin @("bin", "obj", "_snapshots", "Internals", "Apache", "AWS", "Azure", "GCP")
  } | ForEach-Object {
    $nestedTestGroupFile = Join-Path -Path $_.FullName -ChildPath ".testgroup"
    if (-Not (Test-Path -Path $nestedTestGroupFile)) {
      throw "Missing .testgroup file in subdirectory: $($_.FullName) of test project: $($_.FullName)"
    }

    $nestedTestGroupContent = (Get-Content -Path $nestedTestGroupFile -Raw).Trim()

    if ($nestedTestGroupContent -ine 'Disable')
    {
      $nestedTestGroupContent
    }
  }
}

# Revert to $currenctDirectory
Set-Location $currentDirectory
# Convert the collected testgroups to compressed JSON format
$testGroupsJson = $testGroups | Sort-Object -Unique | ConvertTo-Json -AsArray -Compress
$testGroupsJson ?? "[]"

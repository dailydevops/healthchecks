[CmdletBinding()]
param (
  [Parameter(Mandatory = $true)]
  [string]$WorkingDirectory
)

function Get-Packages {
  param (
  )
  $repositoryUrl = 'https://github.com/dailydevops/healthchecks'
  $queryUrl = 'https://azuresearch-usnc.nuget.org/query?q=netevolve.healthchecks&prerelease=true&semVerLevel=2.0.0'
  $response = Invoke-WebRequest -Uri $queryUrl
  if ($response.statuscode -ne 200) {
    Write-Error "Failed to get packages from $repositoryUrl"
    return
  }

  $data = ConvertFrom-Json $response.Content
  if ($data.totalHits -eq 0) {
    Write-Error "No packages found at $repositoryUrl"
    return
  }

  $result = @"

| Package Name | Current Version | Downloads | Description |
|:-------------|:---------------:|:---------:|-------------|

"@

  foreach ($package in ($data.data | Sort-Object -Property id)) {
    if ($package.projectUrl -ne $repositoryUrl) {
      continue
    }

    $result += "| **[$($package.title)](https://www.nuget.org/packages/$($package.id)/)** "
    $result += "| [![NuGet Version](https://img.shields.io/nuget/v/$($package.id)?&logo=nuget)](https://img.shields.io/nuget/v/$($package.id)?logo=nuget)"
    $result += "| [![NuGet Downloads](https://img.shields.io/nuget/dt/$($package.id)?&logo=nuget)](https://img.shields.io/nuget/v/$($package.id)?logo=nuget)"
    $result += "| $($package.description) |`n"

  }

  return $result
}

function Update-Readme {
  param (
    [Parameter(Mandatory = $true)]
    [string] $workingDirectoy,
    [Parameter(Mandatory = $true)]
    [string] $packagesContent
  )

  $tagStart = '<!-- packages:start -->'
  $tagStartLength = $tagStart.Length
  $tagEnd = '<!-- packages:end -->'

  $readmeFiles = Get-ChildItem -Path $workingDirectory -Filter 'README.*' -Recurse

  foreach ($readmeFile in $readmeFiles) {
    $readmeContent = Get-Content -Path $readmeFile.FullName -Raw

    $startIndex = $readmeContent.IndexOf($tagStart)
    if ($startIndex -eq -1) {
      continue
    }
    $startIndex += $tagStartLength
    $endIndex = $readmeContent.IndexOf($tagEnd, $startIndex)

    if ($endIndex -eq -1) {
      continue
    }

    $readmeContent = $readmeContent.Replace($readmeContent.Substring($startIndex, $endIndex - $startIndex), $packagesContent)

    Set-Content -Path $readmeFile.FullName -Value $readmeContent -NoNewline
  }
}

$packages = Get-Packages
if (![string]::IsNullOrWhiteSpace($packages)) {
  Update-Readme -workingDirectoy $WorkingDirectory -packagesContent $packages
}

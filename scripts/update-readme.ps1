[CmdletBinding()]
param (
  [Parameter(Mandatory = $true)]
  [string]$WorkingDirectory
)

function Get-Packages {
  param (
  )
  Write-Host "Getting packages from Nuget"

  $resultData = @()
  $take = 100
  $skip = 0
  $repositoryUrl = 'https://github.com/dailydevops/healthchecks'

  do {
    $queryUrl = "https://azuresearch-usnc.nuget.org/query?q=netevolve.healthchecks&take=$($take)&skip=$($skip)&prerelease=true&semVerLevel=2.0.0"
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

    $skip += $data.data.Length
    $resultData += $data.data

  } while ($skip -lt $data.totalHits)


  $result = @"

| Package Name | NuGet Link      |
|:-------------|:---------------:|

"@

  foreach ($package in ($resultData | Sort-Object -Property id)) {
    if (!$package.projectUrl.StartsWith($repositoryUrl, [System.StringComparison]::OrdinalIgnoreCase)) {
      continue
    }

    $result += "| [$($package.title)](https://www.nuget.org/packages/$($package.id)/) "
    if ($package.deprecation) {
      $result += "❌ **DEPRECATED**"
    }
    $result += "<br/><small>$($package.description)</small> "

    $result += "| [![NuGet Downloads](https://img.shields.io/nuget/dt/$($package.id)?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/$($package.id)/#readme-body-tab) "
    $result += "|`n"

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
  $tagEnd = '<!-- packages:end -->'

  $readmeFiles = Get-ChildItem -Path $workingDirectory -Filter 'README.*' -Recurse

  foreach ($readmeFile in $readmeFiles) {
    $readmeContent = Get-Content -Path $readmeFile.FullName -Raw

    $startIndex = $readmeContent.IndexOf($tagStart)
    if ($startIndex -eq -1) {
      continue
    }
    $startIndex += $tagStart.Length
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

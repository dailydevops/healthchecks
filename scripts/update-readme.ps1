[CmdletBinding()]
param (
  [Parameter(Mandatory = $true)]
  [string]$WorkingDirectory
)

function Get-Packages {
  param (
  )
  $repositoryUrl = 'https://github.com/dailydevops/healthchecks'
  $queryUrl = 'https://api-v2v3search-0.nuget.org/query?q=NetEvolve.HealthChecks'
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

  $result = @'

<table>
  <thead>
    <tr>
      <td><b>Package Name</b></td>
      <td><b>Current Version</b></td>
      <td><b>Downloads</b></td>
    </tr>
  </thead>
  <tbody>

'@

  foreach ($package in ($data.data | Sort-Object -Property id)) {
    if ($package.projectUrl -ne $repositoryUrl) {
      continue
    }

    $description = $package.summary;
    if ([string]::IsNullOrWhiteSpace($description)) {
      $description = $package.description;
    }

    $result += @"
    <tr>
      <td><a href="https://www.nuget.org/packages/$($package.id)/"><b>$($package.title)</b></a></td>
      <td><a href="https://www.nuget.org/packages/$($package.id)/"><img src="https://img.shields.io/nuget/v/$($package.id)?logo=nuget" alt="Nuget"></a></td>
      <td><a href="https://www.nuget.org/packages/$($package.id)/"><img src="https://img.shields.io/nuget/dt/$($package.id)?logo=nuget" alt="Nuget"></a></td>
    </tr>
    <tr>
      <td colspan=3><sub><i>$($description)</i></sub></td>
    </tr>

"@
  }

  $result += @'
  </tbody>
</table>

'@

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

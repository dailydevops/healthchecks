<#
.SYNOPSIS
    Updates README files with the latest NuGet package information from the NetEvolve.HealthChecks repository.

.DESCRIPTION
    This script fetches all published NetEvolve.HealthChecks packages from NuGet.org and updates
    README files throughout the repository with a formatted table containing package details,
    download statistics, and deprecation warnings. The table is inserted between special HTML
    comment tags (<!-- packages:start --> and <!-- packages:end -->).

.PARAMETER WorkingDirectory
    The root directory of the repository where README files will be searched and updated.
    This parameter is mandatory.

.EXAMPLE
    .\Update-Readme.ps1 -WorkingDirectory "C:\src\healthchecks"

    Fetches package information and updates all README files in the specified directory.

.NOTES
    Author: NetEvolve Contributors
    Last Modified: 2025-12-05
#>

[CmdletBinding()]
param (
  [Parameter(Mandatory = $true)]
  [string]$WorkingDirectory
)

<#
.SYNOPSIS
    Retrieves all NetEvolve.HealthChecks packages from NuGet.org and formats them as a Markdown table.

.DESCRIPTION
    Queries the NuGet API to fetch all packages matching "netevolve.healthchecks", including prerelease
    versions. The function handles pagination (100 packages per request) and filters results to only
    include packages from the dailydevops/healthchecks repository. Returns a formatted Markdown table
    with package names, descriptions, deprecation status, and download badges.

.OUTPUTS
    System.String
    A Markdown-formatted table containing package information, or null if retrieval fails.
#>
function Get-Packages {
  param (
  )
  Write-Host "Getting packages from Nuget"

  # Initialize variables for pagination and filtering
  $resultData = @()
  $take = 100  # Number of packages to retrieve per request
  $skip = 0    # Starting index for pagination
  $repositoryUrl = 'https://github.com/dailydevops/healthchecks'

  # Paginate through all available packages
  do {
    # Build query URL with pagination parameters and prerelease inclusion
    $queryUrl = "https://azuresearch-usnc.nuget.org/query?q=netevolve.healthchecks&take=$($take)&skip=$($skip)&prerelease=true&semVerLevel=2.0.0"
    $response = Invoke-WebRequest -Uri $queryUrl

    # Validate response status
    if ($response.statuscode -ne 200) {
      Write-Error "Failed to get packages from $repositoryUrl"
      return
    }

    # Parse JSON response
    $data = ConvertFrom-Json $response.Content
    if ($data.totalHits -eq 0) {
      Write-Error "No packages found at $repositoryUrl"
      return
    }

    # Accumulate results and update pagination offset
    $skip += $data.data.Length
    $resultData += $data.data

  } while ($skip -lt $data.totalHits)

  # Initialize Markdown table header
  $result = @"

| Package Name | NuGet Link      |
|:-------------|:---------------:|

"@

  # Process each package and build table rows
  foreach ($package in ($resultData | Sort-Object -Property id)) {
    # Filter out packages not belonging to this repository
    if (!$package.projectUrl.StartsWith($repositoryUrl, [System.StringComparison]::OrdinalIgnoreCase)) {
      continue
    }

    # Build package name cell with link and description
    $result += "| [$($package.title)](https://www.nuget.org/packages/$($package.id)/) "

    # Add deprecation warning if applicable
    if ($package.deprecation) {
      $result += "❌ **DEPRECATED**"
    }
    $result += "<br/><small>$($package.description)</small> "

    # Build download statistics cell with badge
    $result += "| [![NuGet Downloads](https://img.shields.io/nuget/dt/$($package.id)?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/$($package.id)/#readme-body-tab) "
    $result += "|`n"

  }

  return $result
}

<#
.SYNOPSIS
    Updates all README files in the specified directory with the provided package information.

.DESCRIPTION
    Searches recursively for all README files (README.*) and replaces content between
    <!-- packages:start --> and <!-- packages:end --> tags with the provided package table.
    Only files containing both tags will be modified.

.PARAMETER workingDirectory
    The root directory to search for README files.

.PARAMETER packagesContent
    The Markdown-formatted table content to insert between the tags.

.NOTES
    Files are updated in-place without adding extra newlines at the end.
#>
function Update-Readme {
  param (
    [Parameter(Mandatory = $true)]
    [string] $workingDirectory,
    [Parameter(Mandatory = $true)]
    [string] $packagesContent
  )

  # Define HTML comment tags that mark the injection point
  $tagStart = '<!-- packages:start -->'
  $tagEnd = '<!-- packages:end -->'

  # Find all README files recursively
  $readmeFiles = Get-ChildItem -Path $workingDirectory -Filter 'README.*' -Recurse

  foreach ($readmeFile in $readmeFiles) {
    # Read entire file content as single string
    $readmeContent = Get-Content -Path $readmeFile.FullName -Raw

    # Locate start tag
    $startIndex = $readmeContent.IndexOf($tagStart)
    if ($startIndex -eq -1) {
      # Skip files without start tag
      continue
    }
    # Move index to after the start tag
    $startIndex += $tagStart.Length

    # Locate end tag
    $endIndex = $readmeContent.IndexOf($tagEnd, $startIndex)

    if ($endIndex -eq -1) {
      # Skip files without end tag
      continue
    }

    # Replace content between tags with new package table
    $readmeContent = $readmeContent.Replace($readmeContent.Substring($startIndex, $endIndex - $startIndex), $packagesContent)

    # Write updated content back to file without trailing newline
    Set-Content -Path $readmeFile.FullName -Value $readmeContent -NoNewline
  }
}

# Main execution: Fetch packages and update README files
$packages = Get-Packages
if (![string]::IsNullOrWhiteSpace($packages)) {
  Update-Readme -workingDirectory $WorkingDirectory -packagesContent $packages
}

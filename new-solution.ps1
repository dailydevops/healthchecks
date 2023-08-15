[CmdletBinding()]
param (
    # Name of the solution to be created.
    [Parameter(Mandatory = $true)]
    [string]
    $SolutionName
)

Write-Output "Updating submodules ..."
git submodule update --init --recursive --remote | Out-Null

Write-Output "Creating $SolutionName.sln ..."
$location = Get-Location
. .\eng\scripts\new-solution.ps1

New-Solution -SolutionName $SolutionName -Output $location
Remove-Item -Path "$location\new-solution.ps1" -Force

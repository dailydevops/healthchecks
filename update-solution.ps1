[CmdletBinding()]
param (
)
Write-Output "Updating submodules ..."
git submodule update --init --recursive --remote | Out-Null

. .\eng\scripts\update-solution.ps1

Update-Solution -OutputDirectory (Get-Location)

Write-Output "Reinitializing git repository ..."
git init | Out-Null

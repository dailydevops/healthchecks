[CmdletBinding()]
param (
)
Write-Output "Updating submodules ..."
git submodule update --init --recursive --remote | Out-Null

. .\eng\scripts\update-solution.ps1

Update-Solution -OutputDirectory (Get-Location)

Write-Output "Reinitializing git repository ..."
git init | Out-Null

function Add-Projects($directory, $baseDirectory) {
  $projectFiles = Get-ChildItem -Path $directory -Filter *.csproj -Recurse -File

  foreach ($projectFile in $projectFiles) {
    if ($projectFile.FullName -ccontains " - Backup.") {
      continue
    }

    $solutionFolder = $projectFile.Directory.Parent.FullName.Replace($baseDirectory, "").TrimStart("\")

    dotnet sln .\HealthChecks.sln add ($projectFile.FullName) -s $solutionFolder | Out-Null
  }
}

Write-Output "Updating solution ..."

Add-Projects -directory ".\src" -baseDirectory (Get-Location)

[CmdletBinding()]
param (
  # Parameter help description
  [Parameter(Mandatory = $true)]
  [String]
  $ProjectName,

  # Parameter help description
  [Parameter(Mandatory = $false)]
  [ValidateSet('Library', 'Console', 'WebApi', 'WebApp', 'Function', 'BlazorServer', 'BlazorWasm', 'RazorLibrary', 'xUnit', 'NUnit', 'MSTest', 'Benchmarks')]
  [String]
  $ProjectType = 'Library',

  # Parameter help description
  [Parameter(Mandatory = $false)]
  [String]
  $Framework = 'net8.0',

  # Parameter help description
  [Parameter(Mandatory = $false)]
  [Switch]
  $DisableTests,

  # Parameter help description
  [Parameter(Mandatory = $false)]
  [Switch]
  $DisableUnitTests,

  # Parameter help description
  [Parameter(Mandatory = $false)]
  [Switch]
  $DisableIntegrationTests,

  [Parameter(Mandatory = $false)]
  [Switch]
  $DisableArchitectureTests = $false
)

. .\eng\scripts\new-project.ps1

New-Project `
  -ProjectName $ProjectName `
  -ProjectType $ProjectType `
  -Framework $Framework `
  -DisableTests $DisableTests `
  -DisableUnitTests $DisableUnitTests `
  -DisableIntegrationTests $DisableIntegrationTests `
  -SolutionFile "./HealthChecks.sln" `
  -OutputDirectory (Get-Location) `
  -EnableProjectGrouping $true `
  -EnableAdvProjectGrouping $false `
  -DisableArchitectureTests $DisableArchitectureTests

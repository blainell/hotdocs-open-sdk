Param
(
[string] $NugetServer = "\\edv06nuget01\nuget\packages",
[string] $NugetServerPassword = " "
)
$ErrorActionPreference = "Stop"
  
if (!(Test-Path Env:\GO_PIPELINE_LABEL)){
  throw "GO_PIPELINE_LABEL does not exist"
}
if (!(Test-Path Env:\MAJOR_VERSION)){
  throw "MAJOR_VERSION does not exist"
}
if (!(Test-Path Env:\MINOR_VERSION)){
  throw "MINOR_VERSION does not exist"
}
if (!(Test-Path Env:\NUGET_SERVER)){
  throw "NUGET_SERVER does not exist"
}
if (!(Test-Path Env:\NUGET_SERVER_PASSWORD)){
  throw "NUGET_SERVER_PASSWORD does not exist"
}

$BuildNumber = (Get-Item Env:\GO_PIPELINE_LABEL).Value
$MajorVersion = (Get-Item Env:\MAJOR_VERSION).Value
$MinorVersion = (Get-Item Env:\MINOR_VERSION).Value
$NugetServer = (Get-Item Env:\NUGET_SERVER).Value
$NugetServerPassword = (Get-Item Env:\NUGET_SERVER_PASSWORD).Value

$packageVersion = "$MajorVersion.$MinorVersion.$BuildNumber.0"

$scriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Write-Host "Script root is $scriptRoot"

Import-Module $scriptRoot\BuildFunctions

$binPath = "..\BuildArtifacts"
Write-Host "binPath is $binPath"
Write-Host "scriptRoot is $scriptRoot"
Write-Host "PackageVersion is $PackageVersion"
Write-Host "NuGet server is $NugetServer"

Publish-NugetPackage -SrcPath $binPath -NuGetPath $scriptRoot -PackageVersion $PackageVersion -NuGetServer $NugetServer -NugetServerPassword $NugetServerPassword

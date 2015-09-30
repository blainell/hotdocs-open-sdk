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
if (!(Test-Path Env:\NUGET_API_KEY)){
  throw "NUGET_API_KEY does not exist"
}

$MajorVersion = (Get-Item Env:\MAJOR_VERSION).Value
$MinorVersion = (Get-Item Env:\MINOR_VERSION).Value
$PatchVersion= (Get-Item Env:\PATCH_VERSION).Value
$NugetApiKey = (Get-Item Env:\NUGET_API_KEY).Value

$packageVersion = "$MajorVersion.$MinorVersion.$PatchVersion-beta"

$scriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Write-Host "Script root is $scriptRoot"

Import-Module $scriptRoot\BuildFunctions

$binPath = "..\BuildArtifacts"
Write-Host "binPath is $binPath"
Write-Host "scriptRoot is $scriptRoot"
Write-Host "PackageVersion is $PackageVersion"

Publish-NugetPackage -SrcPath $binPath -NuGetPath $scriptRoot -PackageVersion $PackageVersion -NuGetServer $NugetServer -NugetServerPassword $NugetApiKey
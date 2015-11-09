$ErrorActionPreference = "Stop"
$env:Path += ";C:\Program Files (x86)\WiX Toolset v3.10\bin"
$location = (Get-Item -Path ".\" -Verbose).FullName
	
if ($env:GO_ENVIRONMENT_NAME){
	C:\SonarQube\MSBuild.SonarQube.Runner-1.01\MSBuild.SonarQube.Runner.exe begin /k:$env:SONAR_PROJECT_KEY /n:$env:SONAR_PROJECT_NAME /v:$env:SONAR_PROJECT_VERSION
}
	
./NuGet.exe restore ..\HotDocs.Sdk.sln
msbuild /t:"Build" BuildAndUnitTest.build

if ($env:GO_ENVIRONMENT_NAME){			
	C:\SonarQube\MSBuild.SonarQube.Runner-1.01\MSBuild.SonarQube.Runner.exe end
}	
exit $LastExitCode

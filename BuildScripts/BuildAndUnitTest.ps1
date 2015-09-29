$ErrorActionPreference = "Stop"
	
./NuGet.exe restore ..\HotDocs.Sdk.sln
msbuild /t:"Build" BuildAndUnitTest.build

exit $LastExitCode

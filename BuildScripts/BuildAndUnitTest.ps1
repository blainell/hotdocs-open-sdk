$ErrorActionPreference = "Stop"
	
msbuild /t:"Build" BuildAndUnitTest.build

exit $LastExitCode

function Publish-NugetPackage
{
  Param
  (
    [string]$SrcPath,
    [string]$NugetPath,
    [string]$PackageVersion,
    [string]$NugetServer,
    [string]$NugetServerPassword
  )
    if (!$SrcPath){
    throw "No source path specified"
    }
    if (!$NugetPath){
    throw "No NugetPath specified"
    }
    if (!$PackageVersion){
    throw "No PackageVersion specified"
    }
    if (!$NugetServer){
    throw "No NugetServer specified"
    }
    if (!$NugetServerPassword){
    throw "No NugetServerPassword specified"
    }

    Write-Host "Executing Publish-NugetPackage in path $SrcPath, PackageVersion is $PackageVersion"

    $AllNuspecFiles = Get-ChildItem $SrcPath\*.nuspec
    if ($AllNuspecFiles -eq $null){
        Throw "No NuSpec files found"
    }
    
    Write-Host "all files is $AllNuspecFiles"
    #Remove all previous packed packages in the directory
    $AllNugetPackageFiles = Get-ChildItem $SrcPath\*.nupkg

    Write-Host "Removing old nupkg files"
    foreach ($file in $AllNugetPackageFiles)
    {
	  Write-Host "Removing file $file"
      Remove-Item $file
    }

    Write-Host "Creating new nupkg files"

    foreach ($file in $AllNuspecFiles)
    {
        Write-Host "Modifying file " + $file.FullName
        #save the file for restore
        $backFile = $file.FullName + "._ORI"
        $tempFile = $file.FullName + ".tmp"
        Copy-Item $file.FullName $backFile -Force

        #now load all content of the original file and rewrite modified to the same file
        Get-Content $file.FullName |
        %{$_ -replace '<version>.*</version>', "<version>$PackageVersion</version>" } > $tempFile
        Move-Item $tempFile $file.FullName -force

        #Create the .nupkg from the nuspec file
        $ps = new-object System.Diagnostics.Process
        $ps.StartInfo.Filename = "$NugetPath\nuget.exe"
        $ps.StartInfo.Arguments = "pack `"$file`""
        $ps.StartInfo.WorkingDirectory = $file.Directory.FullName
        $ps.StartInfo.RedirectStandardOutput = $True
        $ps.StartInfo.RedirectStandardError = $True
        $ps.StartInfo.UseShellExecute = $false
        $ps.start()
        if(!$ps.WaitForExit(30000))
        {
            $ps.Kill()
        }
        [string] $Out = $ps.StandardOutput.ReadToEnd();
        [string] $ErrOut = $ps.StandardError.ReadToEnd();
        Write-Host "Nuget pack Output of commandline " + $ps.StartInfo.Filename + " " + $ps.StartInfo.Arguments
        Write-Host $Out
        if ($ErrOut -ne "")
        {
            Write-Error "Nuget pack Errors"
            Write-Error $ErrOut
        }
    }

    $AllNugetPackageFiles = Get-ChildItem $SrcPath\*.nupkg

    Write-Host "Uploading NuPkg files to server"
    
    foreach ($file in $AllNugetPackageFiles)
    {
        #Create the .nupkg from the nuspec file
        $ps = new-object System.Diagnostics.Process
        $ps.StartInfo.Filename = "$NugetPath\nuget.exe"
        $ps.StartInfo.Arguments = "push `"$file`" -s $NugetServer $NugetServerPassword"
        $ps.StartInfo.WorkingDirectory = $file.Directory.FullName
        $ps.StartInfo.RedirectStandardOutput = $True
        $ps.StartInfo.RedirectStandardError = $True
        $ps.StartInfo.UseShellExecute = $false
        $ps.start()
        if(!$ps.WaitForExit(30000))
        {
            $ps.Kill()
        }
        [string] $Out = $ps.StandardOutput.ReadToEnd();
        [string] $ErrOut = $ps.StandardError.ReadToEnd();
        Write-Host "Nuget push Output of commandline " + $ps.StartInfo.Filename + " " + $ps.StartInfo.Arguments
        Write-Host $Out
        if ($ErrOut -ne "")
        {
            Write-Error "Nuget push Errors"
            Write-Error $ErrOut
        }

    }
    
}

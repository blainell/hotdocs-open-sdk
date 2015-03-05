REM This batch file requires ONE (1) parameter:
REM   the path to the local copy of the Open SDK help sources ($/Publications/Projects/OpenSDK)

set ORIGDIR=%CD%
chdir .\Help\Working\Output\HtmlHelp1\html

REM Push docs to Open SDK WebHelp source

REM check out destination files
"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe" checkout /recursive $/Publications/Projects/OpenSDK/html $/Publications/Projects/OpenSDK/api.hhc $/Publications/Projects/OpenSDK/api.hhk

REM copy HTML to destination, fixing stuff up along the way
%ORIGDIR%\tools\rxcopy %ORIGDIR%\tools\rules_htm.txt *.htm %1\html\ > %1\html\copied_files.txt

REM copy TOC and Index to destination
xcopy ..\api.hh? %1\ /Y /Q

REM update FPJ file for RoboHelp
chdir /d %1\html
%ORIGDIR%\tools\buildfpj.exe html.fpj < copied_files.txt
del copied_files.txt

REM add all HTML files (in case new files were added)
"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe" add /recursive $/Publications/Projects/OpenSDK/html

REM check in modified files
"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe" checkin /recursive /noprompt /comment:"Checked in auto-generated Cloud Services help content" $/Publications/Projects/OpenSDK/html $/Publications/Projects/OpenSDK/api.hhc $/Publications/Projects/OpenSDK/api.hhk

chdir /d %ORIGDIR%
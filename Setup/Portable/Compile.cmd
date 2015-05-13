@echo off

echo.
echo Portable AForge.NET Framework all projects Release builder
echo ==========================================================
echo. 
echo This Windows batch file will use Visual Studio 2013 to
echo compile the Release versions of the Portable framework.
echo. 

timeout /T 5

@if "%VS120COMNTOOLS%"=="" goto error_no_VS120COMNTOOLSDIR
@call "%VS120COMNTOOLS%VsDevCmd.bat"

@cd "..\..\Sources"
@msbuild "Shim Drawing.sln" /t:Rebuild /p:Configuration=Release;Platform="Any CPU"
@msbuild "Portable AForge.sln" /t:Rebuild /p:Configuration=Release;Platform="Any CPU"

@echo .mdb >> exclude.txt

@set PUBLISH=..\Setup\Portable\Publish\
@set AFORGEDIR=%PUBLISH%\aforge\
@set DRAWDIR=%PUBLISH%\shim_drawing\
@set APCLDIR=%AFORGEDIR%lib\portable-net45+netcore45+wpa81\
@set AWPFDIR=%AFORGEDIR%lib\net45\
@set AUNIDIR=%AFORGEDIR%lib\portable-win81+wpa81\
@set DPCLDIR=%DRAWDIR%lib\portable-net45+netcore45+wpa81\
@set DWPFDIR=%DRAWDIR%lib\net45\

@if EXIST "%PUBLISH%" (rd /s /q "%PUBLISH%")

@md "%AFORGEDIR%"
@xcopy /k /r /v /y ..\README.md "%AFORGEDIR%"
@xcopy /k /r /v /y ..\gpl-3.0.txt "%AFORGEDIR%"
@xcopy /k /r /v /y ..\lgpl-3.0.txt "%AFORGEDIR%"

@md "%DRAWDIR%"
@xcopy /k /r /v /y ..\README.md "%DRAWDIR%"
@xcopy /k /r /v /y ..\gpl-3.0.txt "%DRAWDIR%"
@xcopy /k /r /v /y System.Drawing\CPOL_PixelFormatConversion.htm "%DRAWDIR%"

@md "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Core\bin\Release\AForge.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Fuzzy\bin\Release\AForge.Fuzzy.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Genetic\bin\Release\AForge.Genetic.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Imaging\bin\Release\AForge.Imaging.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Imaging.Formats\bin\Release\AForge.Imaging.Formats.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt MachineLearning\bin\Release\AForge.MachineLearning.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Math\bin\Release\AForge.Math.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Neuro\bin\Release\AForge.Neuro.* "%APCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Vision\bin\Release\AForge.Vision.* "%APCLDIR%"

@md "%AWPFDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Core\_Net\bin\Release\AForge.* "%AWPFDIR%"

@md "%AUNIDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt Core\_Universal\bin\Release\AForge.* "%AUNIDIR%"

@md "%DPCLDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\bin\Release\Shim.Drawing.* "%DPCLDIR%"

@md "%DWPFDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_WPF\bin\Release\Shim.Drawing.* "%DWPFDIR%"

@del /f exclude.txt

@goto end

@REM -----------------------------------------------------------------------
:error_no_VS120COMNTOOLSDIR
@echo ERROR: Cannot determine the location of the VS Common Tools folder.
@goto end

:end
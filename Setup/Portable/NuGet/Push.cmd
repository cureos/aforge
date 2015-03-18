@echo off

echo.
echo Portable AForge.NET Framework NuGet package publisher
echo =========================================================
echo. 
echo This Windows batch file uses NuGet to automatically
echo push the Portable AForge.NET Framework packages to the gallery.
echo. 

timeout /T 5

:: Directory settings
set output=..\bin\nupkg
set current=%~dp0

echo.
echo Current directory: %current%
echo Output  directory: %output%
echo.

forfiles /p %output% /m *.nupkg /c "cmd /c %current%\NuGet.exe push @file"

pause
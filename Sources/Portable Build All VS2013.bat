@if "%VS120COMNTOOLS%"=="" goto error_no_VS120COMNTOOLSDIR
@call "%VS120COMNTOOLS%VsDevCmd.bat"

@cd "%HOMEDRIVE%%HOMEPATH%\Documents\Visual Studio 2012\Projects\aforge\Sources"
@msbuild "Portable Build All.sln" /t:Rebuild /p:Configuration=Release;Platform="Any CPU"

@echo .pri > exclude.txt
@echo .mdb >> exclude.txt

@if EXIST Publish (rd /s /q Publish)

@md Publish
@xcopy /k /r /v /y ..\README.md Publish
@xcopy /k /r /v /y ..\gpl-3.0.txt Publish
@xcopy /k /r /v /y ..\lgpl-3.0.txt Publish
@xcopy /k /r /v /y ..\gpl-3.0.txt Publish
@xcopy /k /r /v /y System.Drawing\CPOL_PixelFormatConversion.htm Publish

@md "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Core\bin\Release\AForge.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Fuzzy\bin\Release\AForge.Fuzzy.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Genetic\bin\Release\AForge.Genetic.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Imaging\bin\Release\AForge.Imaging.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Imaging.Formats\bin\Release\AForge.Imaging.Formats.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt MachineLearning\bin\Release\AForge.MachineLearning.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Math\bin\Release\AForge.Math.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Neuro\bin\Release\AForge.Neuro.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Vision\bin\Release\AForge.Vision.* "Publish\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\bin\Release\Shim.Drawing.* "Publish\PCL\Any CPU"

@md "Publish\Universal\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Universal\bin\Release\Shim.Drawing.* "Publish\Universal\Any CPU"

@md "Publish\Win8\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Store\bin\Release\Shim.Drawing.* "Publish\Win8\Any CPU"

@md "Publish\WP8\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Phone\Bin\Release\Shim.Drawing.* "Publish\WP8\Any CPU"

@md "Publish\WPF\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_WPF\bin\Release\Shim.Drawing.* "Publish\WPF\Any CPU"

@del /f exclude.txt

@goto end

@REM -----------------------------------------------------------------------
:error_no_VS120COMNTOOLSDIR
@echo ERROR: Cannot determine the location of the VS Common Tools folder.
@goto end

:end
@if "%VS120COMNTOOLS%"=="" goto error_no_VS120COMNTOOLSDIR
@call "%VS120COMNTOOLS%VsDevCmd.bat"

@cd "%HOMEDRIVE%%HOMEPATH%\Documents\Visual Studio 2012\Projects\aforge\Sources"
@msbuild "Portable Build All.sln" /t:Rebuild /p:Configuration=Release;Platform="Any CPU"

@echo .pri > exclude.txt
@echo .mdb >> exclude.txt

@if EXIST Publish (rd /s /q Publish)

@md Publish\aforge
@xcopy /k /r /v /y ..\README.md Publish\aforge
@xcopy /k /r /v /y ..\gpl-3.0.txt Publish\aforge
@xcopy /k /r /v /y ..\lgpl-3.0.txt Publish\aforge

@md Publish\shim_drawing
@xcopy /k /r /v /y ..\README.md Publish\shim_drawing
@xcopy /k /r /v /y ..\gpl-3.0.txt Publish\shim_drawing
@xcopy /k /r /v /y System.Drawing\CPOL_PixelFormatConversion.htm Publish\shim_drawing

@md "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Core\bin\Release\AForge.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Fuzzy\bin\Release\AForge.Fuzzy.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Genetic\bin\Release\AForge.Genetic.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Imaging\bin\Release\AForge.Imaging.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Imaging.Formats\bin\Release\AForge.Imaging.Formats.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt MachineLearning\bin\Release\AForge.MachineLearning.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Math\bin\Release\AForge.Math.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Neuro\bin\Release\AForge.Neuro.* "Publish\aforge\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Vision\bin\Release\AForge.Vision.* "Publish\aforge\PCL\Any CPU"

@md "Publish\aforge\Net\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Core\_Net\bin\Release\AForge.* "Publish\aforge\Net\Any CPU"

@md "Publish\aforge\WP8\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Core\_Phone\bin\Release\AForge.* "Publish\aforge\WP8\Any CPU"

@md "Publish\aforge\Universal\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt Core\_Universal\bin\Release\AForge.* "Publish\aforge\Universal\Any CPU"

@md "Publish\shim_drawing\PCL\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\bin\Release\Shim.Drawing.* "Publish\shim_drawing\PCL\Any CPU"

@md "Publish\shim_drawing\WP8\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Phone\Bin\Release\Shim.Drawing.* "Publish\shim_drawing\WP8\Any CPU"

@md "Publish\shim_drawing\WPF\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_WPF\bin\Release\Shim.Drawing.* "Publish\shim_drawing\WPF\Any CPU"

@del /f exclude.txt

@goto end

@REM -----------------------------------------------------------------------
:error_no_VS120COMNTOOLSDIR
@echo ERROR: Cannot determine the location of the VS Common Tools folder.
@goto end

:end
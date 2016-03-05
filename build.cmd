@ECHO OFF

:: Import VS tools
IF DEFINED VS140COMNTOOLS (
  CALL "%VS140COMNTOOLS%vsvars32.bat"
) ELSE (
  IF DEFINED VS120COMNTOOLS (
    CALL "%VS120COMNTOOLS%vsvars32.bat"
  ) ELSE (
    IF DEFINED VS110COMNTOOLS (
      CALL "%VS110COMNTOOLS%vsvars32.bat"
    ) ELSE (
      EXIT /B 1
    )
  )
)

:: Init workspace
IF EXIST init.cmd (
  CALL init.cmd
)

:: Download NuGet
powershell -NoProfile -ExecutionPolicy Bypass -Command "(new-object System.Net.WebClient).DownloadFile('https://nuget.org/nuget.exe', '.\NuGet.exe')"

:: Install packages we need
nuget install gitversion -SolutionDir .\Qwiq -verbosity quiet -excludeversion
nuget restore .\Qwiq\Qwiq.sln -NonInteractive -DisableParallelProcessing

:: Delete NuGet
DEL nuget.exe

:: Output version information for CI
gitversion /output buildserver

:: Build
msbuild .\Qwiq\Qwiq.sln /nologo /clp:"NoSummary;Verbosity=minimal" /m /p:Configuration=Release /t:Rebuild

:: If build didn't work, bail
IF ERRORLEVEL 1 (
  EXIT /B %ERRORLEVEL%
) 

:: Package up our .csproj items into packages
powershell -noprofile -executionpolicy bypass -command "gci -Filter *.csproj -Recurse -Exclude *.Tests.csproj | %% { nuget pack $_.FullName -IncludeReferencedProjects -Symbols -Properties Configuration=Release }"
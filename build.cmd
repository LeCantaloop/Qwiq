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

SET PATH=%PATH%;%CD%

:: Install packages we need
nuget install gitversion.commandline -SolutionDir .\Qwiq -verbosity quiet -excludeversion
nuget restore .\Qwiq\Qwiq.sln -NonInteractive -DisableParallelProcessing

:: Delete NuGet
DEL nuget.exe

:: Output version information for CI
Qwiq\packages\GitVersion.CommandLine\tools\gitversion /output buildserver

:: Build
msbuild .\Qwiq\Qwiq.sln /nologo /clp:"NoSummary;Verbosity=minimal" /m /p:Configuration=Release /t:Rebuild
IF ERRORLEVEL 1 (
  EXIT /B %ERRORLEVEL%
)

:: Build for all target frameworks
powershell -noprofile -executionpolicy bypass -command "$v = @('4.5','4.5.1','4.5.2','4.6','4.6.1','4.6.2'); gci -Filter *.csproj -Recurse -Exclude *.Tests.csproj | %% { foreach ($version in $v) { $lib = ($version.replace('.', '')); & msbuild $_ /nologo /clp:Verbosity=minimal /m /p:Configuration=Release /p:TargetFrameworkVersion=$version /p:OutputPath=bin\Release\net$lib\ /t:Rebuild } }"
IF ERRORLEVEL 1 (
  EXIT /B %ERRORLEVEL%
)

:: Package up our .csproj items into packages
powershell -noprofile -executionpolicy bypass -command "gci -Filter *.csproj -Recurse -Exclude *.Tests.csproj | %% { nuget pack $_.FullName -IncludeReferencedProjects -Symbols -Properties Configuration=Release }"

powershell -noprofile -executionpolicy bypass -command ".\init.ps1;.\scripts\Test.ps1 -repoRoot ."
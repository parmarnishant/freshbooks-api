@ECHO OFF
REM ===========================================================================
REM Generates the source and builds the application
REM ===========================================================================
@REM 	Must be run from the current directory
@CD /d %~dp0

SET BuildConfig=%1
IF "%BuildConfig%" == "" SET BuildConfig=Debug

depend\CmdTool.exe build src\*.csproj
IF NOT "%ERRORLEVEL%" == "0" GOTO ERROR

C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe /nologo /v:m /target:Rebuild /p:Configuration=%BuildConfig% "/p:Platform=Any CPU" /toolsversion:3.5 /l:FileLogger,Microsoft.Build.Engine;logfile=MSBuild.log;append=true;verbosity=diagnostic;encoding=utf-8 src\Freshbooks.sln
IF NOT "%ERRORLEVEL%" == "0" GOTO ERROR

"%VS90COMNTOOLS%\..\IDE\mstest.exe" /nologo /noisolation /noresults /testcontainer:src\Freshbooks.Test\bin\Debug\Freshbooks.Test.dll
IF NOT "%ERRORLEVEL%" == "0" GOTO ERROR

SET BuildConfig=
GOTO EXIT

REM ===========================================================================
:ERROR
REM ===========================================================================
ECHO.
ECHO Build Failed.
SET BuildConfig=
EXIT /B 1

REM ===========================================================================
:EXIT
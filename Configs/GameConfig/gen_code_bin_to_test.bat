@echo off
Cd /d %~dp0
echo %CD%

set WORKSPACE=../..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.
set DATA_OUTPATH=%WORKSPACE%/Configs/GameConfig/Test/Data/

dotnet %LUBAN_DLL% ^
    -t client ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%DATA_OUTPATH%

if not defined AI_MODE pause
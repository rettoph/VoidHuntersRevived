@ECHO OFF
set profile=%1

WHERE mgfxc
IF %ERRORLEVEL% NEQ 0 (
	ECHO mgfxc wasn't found, please install it from https://docs.monogame.net/articles/tools/mgfxc.html
	GOTO :eof
) ELSE (
	IF NOT DEFINED profile GOTO :promptProfile
	GOTO :build
)

:promptProfile
ECHO Enter desired profile...
ECHO See 'mgfxc /help' for more info.
set /p profile=

:build
FOR /f %%i in ('@findstr /i "\/Platform:" Content.mgcb') DO SET platform=%%i
SET platform=%platform:/Platform:=%
if not exist "Effects\Compiled" mkdir Effects\Compiled

FOR %%I IN (Effects\*.fx) DO (
	mgfxc %%I Effects\Compiled\%%~nI.mgfx /Profile:%profile%
)

:eof
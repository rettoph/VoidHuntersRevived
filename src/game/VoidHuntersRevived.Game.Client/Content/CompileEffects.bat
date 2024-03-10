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
ECHO Enter desired profile or press enter for default (OpenGL)...
ECHO See 'mgfxc /help' for more info.
set profile=OpenGL
set /p profile=

:build
echo Using Profile: %profile%
FOR /f %%i in ('@findstr /i "\/Platform:" Content.mgcb') DO SET platform=%%i
SET platform=%platform:/Platform:=%
if not exist "Client\Effects\Compiled" mkdir Client\Effects\Compiled

FOR %%I IN (Client\Effects\*.fx) DO (
	mgfxc %%I Client\Effects\Compiled\%%~nI.mgfx /Profile:%profile%
)
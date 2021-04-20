@echo ON

set rids=win10-x64 win10-x86 linux-x64
set version=0.1.6

mkdir build\%version%

Rem Build Installers
"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" src\installers\VoidHuntersRevived.Installer.Windows\VoidHuntersRevived.Installer.Windows.wixproj /p:Configuration=Release /p:PlatformTarget=x64 /p:OutputPath=%cd%\temp
move temp\en-us\VoidHuntersRevived.Installer.Windows.msi build\%version%
rename build\%version%\VoidHuntersRevived.Installer.Windows.msi vhr_0.1.6_installer_win-x64.msi
rmdir /Q /S temp

"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" src\installers\VoidHuntersRevived.Installer.Windows\VoidHuntersRevived.Installer.Windows.wixproj /p:Configuration=Release /p:PlatformTarget=x86 /p:OutputPath=%cd%\temp
move temp\en-us\VoidHuntersRevived.Installer.Windows.msi build\%version%
rename build\%version%\VoidHuntersRevived.Installer.Windows.msi vhr_0.1.6_installer_win-x86.msi
rmdir /Q /S temp

(for %%r in (%rids%) do ( 
   echo %%r
   
   	Rem Build Launcher...
	dotnet publish "src\utilities\VoidHuntersRevived.Utilities.Launcher" --runtime %%r --self-contained true --output "temp" --configuration Release
	tar.exe -C temp -a -c -f build/%version%/vhr_%version%_launcher_%%r.zip *
	rmdir /Q /S temp
	
	Rem Build Client Launcher...
	dotnet publish "src\clients\VoidHuntersRevived.Client.Launcher" --runtime %%r --self-contained true --output "temp" --configuration Release
	tar.exe -C temp -a -c -f build/%version%/vhr_%version%_client-launcher_%%r.zip *
	rmdir /Q /S temp
	
	Rem Build Client...
	dotnet publish "src\clients\VoidHuntersRevived.Client.Desktop" --runtime %%r --self-contained true --output "temp"
	tar.exe -C temp -a -c -f build/%version%/vhr_%version%_desktop_%%r.zip *
	rmdir /Q /S temp
	
	Rem Build Builder...
	dotnet publish "src\clients\VoidHuntersRevived.Builder" --runtime %%r --self-contained true --output "temp"
	tar.exe -C temp -a -c -f build/%version%/vhr_%version%_builder_%%r.zip *
	rmdir /Q /S temp
	
	Rem Build Server...
	dotnet publish "src\VoidHuntersRevived.Server" --runtime %%r --self-contained true --output "temp"
	tar.exe -C temp -a -c -f build/%version%/vhr_%version%_server_%%r.zip *
	rmdir /Q /S temp
))

pause
@echo OFF

set rids=win10-x64 win10-x86 linux-x64
set version=0.1.5

mkdir build\%version%

(for %%r in (%rids%) do ( 
   echo %%r
   
   	Rem Build Launcher...
	dotnet publish "src\external\VoidHuntersRevived.Client.Launcher" --runtime %%r --self-contained true --output "temp" --configuration Release
	tar.exe -C temp -a -c -f build/%version%/vhr_%version%_launcher_%%r.zip *
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
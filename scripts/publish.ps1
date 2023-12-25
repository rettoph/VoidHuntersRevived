Import-Module -Name ($PSScriptRoot + "\modules\publish-module.ps1") -Force

Publish-VoidHunters Client Release win_x64 $true $true $true $true
Publish-VoidHunters Server Release win_x64 $true $true $true
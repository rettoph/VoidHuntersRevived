Param(
    [Configuration]$configuration=[Configuration]::Release,
    [RuntimeIdentifier]$rid=[RuntimeIdentifier]::win_x64,
    [bool]$selfContained = $true,
    [bool]$singleFile = $true,
    [bool]$cleanup = $true
)

Import-Module -Name ($PSScriptRoot + "\shared\source-utils.ps1") -Force
Import-Module -Name ($PSScriptRoot + "\shared\publish-utils.ps1") -Force

PublishVoidHunters Client $configuration $rid $selfContained $singleFile $cleanup
PublishVoidHunters Server $configuration $rid $selfContained $singleFile $cleanup
Param(
    $configuration = "Release",
    $rid = "win_x64",
    [bool]$selfContained = $true,
    [bool]$singleFile = $false,
    [bool]$cleanup = $false
)

Import-Module -Name ($PSScriptRoot + "\shared\source-utils.ps1") -Force
Import-Module -Name ($PSScriptRoot + "\shared\publish-utils.ps1") -Force

PublishVoidHunters Client $configuration $rid $selfContained $singleFile $cleanup
PublishVoidHunters Server $configuration $rid $selfContained $singleFile $cleanup
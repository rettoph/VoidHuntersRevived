Import-Module -Name ($PSScriptRoot + "\source-utils.ps1") -Force

<#
    https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
    RID is short for Runtime Identifier. RID values are used to identify target platforms where the application runs. 
    They're used by .NET packages to represent platform-specific assets in NuGet packages. 
    The following values are examples of RIDs: linux-x64, ubuntu.14.04-x64, win7-x64, or osx.10.12-x64. 
    For the packages with native dependencies, the RID designates on which platforms the package can be restored.
#>
[Flags()]enum RuntimeIdentifier
{
    win_x64 = 1
    win_x86 = 2
    linux_x64 = 4 # (Most desktop distributions like CentOS, Debian, Fedora, Ubuntu, and derivatives)
    linux_arm = 8 # (Linux distributions running on Arm like Raspbian on Raspberry Pi Model 2+)
    linux_arm64 = 16 # (Linux distributions running on 64-bit Arm like Ubuntu Server 64-bit on Raspberry Pi Model 3+)
}

[Flags()]enum Configuration
{
    Release
    Debug
}

function RuntimeIdentifierString()
{
    Param
    (
        [RuntimeIdentifier]$rids    
    )

    return $rids.ToString().Replace("_", "-").Replace(", ", ";")
}

function PublishVoidHunters()
{
    Param
    (
        [Parameter(Mandatory=$true)][Project]$project, 
        [Configuration]$configuration = [Configuration]::Debug,
        [RuntimeIdentifier]$runtime = [RuntimeIdentifier]::win_x64,
        [bool]$selfContained = $false,
        [bool]$singleFile = $false,
        [bool]$cleanup = $false
    )

    $path = GetProjectPath $project
    $rid = RuntimeIdentifierString $runtime
    $output = $PSScriptRoot + "\..\..\publish\" + $rid + "\" + $project

    Remove-Item $output\* -Recurse -Force

    $build = "dotnet publish $path -c $configuration -r $rid -p:PublishSingleFile=$singleFile --self-contained $selfContained -o $output"
    Invoke-Expression $build

    if($cleanup)
    {
        Remove-Item $output\*.pdb -Recurse -Force
        Remove-Item $output\*.xml -Recurse -Force
    }
}
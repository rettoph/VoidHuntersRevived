Import-Module -Name ($PSScriptRoot + "\source-module.ps1") -Force

<#
    https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
    RID is short for Runtime Identifier. RID values are used to identify target platforms where the application runs. 
    They're used by .NET packages to represent platform-specific assets in NuGet packages. 
    The following values are examples of RIDs: linux-x64, ubuntu.14.04-x64, win7-x64, or osx.10.12-x64. 
    For the packages with native dependencies, the RID designates on which platforms the package can be restored.
#>
[Flags()]enum VoidHuntersRuntimeIdentifier
{
    win_x64 = 1
    win_x86 = 2
    linux_x64 = 4 # (Most desktop distributions like CentOS, Debian, Fedora, Ubuntu, and derivatives)
    linux_arm = 8 # (Linux distributions running on Arm like Raspbian on Raspberry Pi Model 2+)
    linux_arm64 = 16 # (Linux distributions running on 64-bit Arm like Ubuntu Server 64-bit on Raspberry Pi Model 3+)
}

[Flags()]enum VoidHuntersConfiguration
{
    Release
    Debug
}

function VoidHuntersRuntimeIdentifierString()
{
    Param
    (
        [VoidHuntersRuntimeIdentifier]$rids    
    )

    return $rids.ToString().Replace("_", "-").Replace(", ", ";")
}

function Publish-VoidHunters()
{
    Param
    (
        [Parameter(Mandatory=$true)][VoidHuntersProject]$project, 
        [VoidHuntersConfiguration]$configuration = [VoidHuntersConfiguration]::Debug,
        [VoidHuntersRuntimeIdentifier]$runtime = [VoidHuntersRuntimeIdentifier]::win_x64,
        [bool]$selfContained = $false,
        [bool]$singleFile = $false,
        [bool]$cleanup = $false,
        [bool]$zip = $false
    )

    $path = GetVoidHuntersProjectPath $project
    $rid = VoidHuntersRuntimeIdentifierString $runtime
    $directory = $PSScriptRoot + "\..\..\publish\" + $project + "\" + $configuration + "\" + $rid + "\"

	if(Test-Path $directory)
	{
		Remove-Item $directory\* -Recurse -Force
	}

    $build = "dotnet publish $path -c $configuration -r $rid -p:PublishSingleFile=$singleFile --self-contained $selfContained -o $directory"
    & Invoke-Expression $build | Out-Null
    	
    if($cleanup)
    {
        Remove-Item $directory\*.pdb -Recurse -Force
        Remove-Item $directory\*.xml -Recurse -Force
    }

    if($zip)
    {
        $zipPath = $PSScriptRoot + "\..\..\publish\VoidHuntersRevived_" + ($project.ToString() + "_" + $configuration + "_" + $rid).ToLower() + ".zip"
        if(Test-Path $zipPath)
        {
            Remove-Item $zipPath -Force
        }
        
        Compress-Archive ($directory + "\*") -DestinationPath $zipPath
        
        return $zipPath 
    }

    return Resolve-Path $directory
}
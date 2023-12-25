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
        [bool]$addLoggerConditionals = $true,
        [bool]$logOutput = $true
    )

    $path = GetVoidHuntersProjectPath $project
    $rid = VoidHuntersRuntimeIdentifierString $runtime
    $directory = $PSScriptRoot + "\..\..\publish\" + $rid + "\" + $configuration + "\" + $project + "\"

	if(Test-Path $directory)
	{
		Remove-Item $directory\* -Recurse -Force
	}

    if($addLoggerConditionals)
    {
        Write-Information $SourcePath
        $files = Get-ChildItem $SourcePath -Recurse -Include "*.cs"
        foreach ($file in $files)
        {
            $content = Get-Content -Path $file.FullName -Raw
            $contentMatches = [regex]::Matches($content, "(?<!#if DEBUG\n)(?<!#if DEBUG\r\n)^( |\t|)*.*logger\.(Verbose|Debug)\(.*?\);(\n|\r\n)", [Text.RegularExpressions.RegexOptions]'Multiline')
            
            if($contentMatches.Count -gt 0)
            {
                $start = "#if DEBUG`n";
                $end = "#endif`n"
                $index = 0;
                $newContent = $content;
                foreach($contentMatch in $contentMatches)
                {
                    if($logOutput)
                    {
                        "Adding conditionals to  logger.Verbose invocation in " + $file.Name + " at " + $contentMatch.Index + ", '" + $contentMatch.Value + "'"
                    }
                    $offset = $contentMatch.Index + (($start.Length + $end.Length) * $index);
                    $newContent = $newContent.Insert($offset, $start)
                    $newContent = $newContent.Insert($offset + $start.Length + $contentMatch.Length, $end);

                    $index = $index + 1;
                }

                Set-Content -Path $file.FullName -Value $newContent
            }
        }
    }

    $build = "dotnet publish $path -c $configuration -r $rid -p:PublishSingleFile=$singleFile --self-contained $selfContained -o $directory"
    if($logOutput)
    {
        Invoke-Expression $build
    }
    else 
    {
        $output = & Invoke-Expression $build
    }
    
	
    if($cleanup)
    {
        Remove-Item $directory\*.pdb -Recurse -Force
        Remove-Item $directory\*.xml -Recurse -Force
    }

    return Resolve-Path $directory
}
Import-Module -Name ($PSScriptRoot + "\refresh-modules.ps1") -Force

###############################################
#                CONFIGURATION                #
###############################################
$projects = @( [VoidHuntersProject]::Client, [VoidHuntersProject]::Server )
$rids = @( [VoidHuntersRuntimeIdentifier]::win_x64, [VoidHuntersRuntimeIdentifier]::linux_x64, [VoidHuntersRuntimeIdentifier]::linux_arm64 )
$configuration = [VoidHuntersConfiguration]::Release
$selfContained = $true
$singleFile = $true
$cleanup = $true
$zip = $true
$resetPublishFolder = $true

###############################################
#                 DANGER ZONE                 #
###############################################
if($resetPublishFolder -eq $true)
{
    $publishDirectory = $PSScriptRoot + "\..\publish"
    if(Test-Path $publishDirectory)
    {
        Remove-Item $publishDirectory\* -Recurse -Force
    }
}

if($configuration -eq [VoidHuntersConfiguration]::Release)
{
    $editor = [VoidHuntersEditor]::new()

    # Remove all Verbose logger calls
    $editor.RemoveAll('^( |\t)*?.*?(l|L)ogger\.Verbose\(.*?\);\s*$')
}

dotnet build-server shutdown
foreach($project in $projects)
{
    foreach($rid in $rids)
    {
        Publish-VoidHunters -project $project -configuration $configuration -runtime $rid -selfContained $selfContained -singleFile $singleFile -cleanup $cleanup -zip $zip
    }
}

if($configuration -eq [VoidHuntersConfiguration]::Release)
{
    $editor.Reset()
}
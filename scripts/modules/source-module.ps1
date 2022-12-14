$SourcePath = $PSScriptRoot + "\..\..\src\"

enum VoidHuntersProject
{
    Client
    Server
}

function GetVoidHuntersProjectPath
{
    Param(
        [VoidHuntersProject]$project
    )

    if($project -eq [VoidHuntersProject]::Client)
    {
        return $SourcePath + "clients\VoidHuntersRevived.Client.Desktop"
    }

    if($project -eq [VoidHuntersProject]::Server)
    {
        return $SourcePath + "VoidHuntersRevived.Server"
    }

}
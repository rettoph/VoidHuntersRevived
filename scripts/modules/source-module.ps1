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

    if($project -eq [Project]::Client)
    {
        return $SourcePath + "clients\VoidHuntersRevived.Client.Desktop"
    }

    if($project -eq [Project]::Server)
    {
        return $SourcePath + "VoidHuntersRevived.Server"
    }

}
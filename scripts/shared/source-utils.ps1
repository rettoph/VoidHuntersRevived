$SourcePath = $PSScriptRoot + "\..\..\src\"

enum Project
{
    Client
    Server
}

function GetProjectPath
{
    Param(
        [Project]$project
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
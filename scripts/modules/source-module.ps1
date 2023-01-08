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
        return $SourcePath + "VoidHuntersRevived.Application.Client"
    }

    if($project -eq [VoidHuntersProject]::Server)
    {
        return $SourcePath + "VoidHuntersRevived.Application.Server"
    }

}
$SourcePath = $PSScriptRoot + "\..\..\src\"

enum VoidHuntersProject
{
    Client
    DummyClient
    Server
}

function GetVoidHuntersProjectPath
{
    Param(
        [VoidHuntersProject]$project
    )

    if($project -eq [VoidHuntersProject]::Client)
    {
        return $SourcePath + "VoidHuntersRevived.Presentation.Client"
    }
	
	if($project -eq [VoidHuntersProject]::DummyClient)
    {
        return $SourcePath + "VoidHuntersRevived.Presentation.DummyClient"
    }

    if($project -eq [VoidHuntersProject]::Server)
    {
        return $SourcePath + "VoidHuntersRevived.Presentation.Server"
    }

}
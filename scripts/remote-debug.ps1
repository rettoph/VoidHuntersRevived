# Load required modules
Import-Module -Name ($PSScriptRoot + "\modules\publish-module.ps1") -Force
if((Get-Module -ListAvailable -Name SomeModule) -eq 0)
{
    Install-Module -Name Posh-SSH
}

# Check config...
$configFile = $PSScriptRoot + "\remote-debug.config.json"
if((Test-Path -Path $configFile -PathType Leaf))
{   
    $config = Get-Content $configFile | ConvertFrom-Json
    "Loaded " + $configPath
}
else 
{
    $config = @{
        Remote = @{
            Host = "";
            Username = "";
            Password = "";
            RelativeFolder = "/VoidHuntersRevived";
        };
        Build = @{
            Project = "Server";
            Configuration = "Debug";
            Runtime = "linux_arm64"
            SelfContained = 1
        }
    }

    New-Item $configFile
    Set-Content $configFile ($config | ConvertTo-Json)

    "No valid configuration file found. Please update the one at " + $configPath

    Exit
}

# Build locally...
"Building Project: '" + $config.Build.Project + "' - " + $config.Build.Runtime
$path = Publish-VoidHunters -project $config.Build.Project -configuration $config.Build.Configuration -runtime $config.Build.Runtime -selfContained $config.Build.SelfContained
$leaf = Split-Path $path -Leaf

# Connect to server...
"Establishing connection with " + $config.Remote.Host
$password = ConvertTo-SecureString $config.Remote.Password -AsPlainText -Force
$credentials = New-Object System.Management.Automation.PSCredential ($config.Remote.Username, $password)
$sftp = New-SFTPSession -ComputerName $config.Remote.Host -Credential $credentials
$ssh = New-SSHSession -ComputerName $config.Remote.Host -Credential $credentials
$destination = (Get-SFTPLocation -SessionId $sftp.SessionId) + $config.Remote.RelativeFolder

"Killing any running tmux session..."
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "tmux kill-session -t VoidHuntersRevived" | Out-Null

# Ensure VoidHuntersRevived folder exists...
if((Test-SFTPPath -SessionId $sftp.SessionId -Path $destination) -eq 0)
{
    "Creating $destination directory..."
    New-SFTPItem -SessionId $sftp.SessionId -Path $destination -ItemType Directory
}

# Delete any old code...
# TODO: Only upload & overwrite new code somehow?
if((Test-SFTPPath -SessionId $sftp.SessionId -Path $destination/$leaf))
{
    "Deleting old version..."
    Remove-SFTPItem -SessionId $sftp.SessionId -Path $destination/$leaf -Force
}

# Upload...
"Uploading files..."
Set-SFTPItem -SessionId $sftp.SessionId -Destination $destination -Path $path -Force

# Start process...
"Starting process via tmux"
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "sudo chmod +x $destination/$leaf/VoidHuntersRevived.Server" | Out-Null
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "tmux new-session -d -s VoidHuntersRevived 'cd $destination/$leaf && ./VoidHuntersRevived.Server'" | Out-Null

# Disconnect from server...
"Closing connection..."
Remove-SSHSession -SessionId $ssh.SessionId | Out-Null
Remove-SFTPSession -SFTPSession $sftp | Out-Null

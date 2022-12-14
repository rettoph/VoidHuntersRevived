#Install-Module -Name Posh-SSH
Import-Module -Name ($PSScriptRoot + "\modules\publish-module.ps1") -Force

# Build locally...
Write-Output "Building Project..."
$path = Publish-VoidHunters -project Server -configuration Debug -runtime linux_arm64 -selfContained 1
$leaf = Split-Path $path -Leaf

# Connect to server...
Write-Output "Creating new SFTP session..."
$password = ConvertTo-SecureString "password" -AsPlainText -Force
$credentials = New-Object System.Management.Automation.PSCredential ("anthony", $password)
$sftp = New-SFTPSession -ComputerName 192.168.0.11 -Credential $credentials
$destination = (Get-SFTPLocation -SessionId $sftp.SessionId) + '/VoidHuntersRevived'

Write-Output "Creating new SSH session..."
$ssh = New-SSHSession -ComputerName 192.168.0.11 -Credential $credentials

Write-Output "Killing any running tmux session..."
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "tmux kill-session -t VoidHuntersRevived" | Out-Null

# Ensure VoidHuntersRevived folder exists...
if((Test-SFTPPath -SessionId $sftp.SessionId -Path $destination) -eq 0)
{
    Write-Output "Creating $destination directory..."
    New-SFTPItem -SessionId $sftp.SessionId -Path $destination -ItemType Directory
}

# Delete any old code...
# TODO: Only upload & overwrite new code somehow?
if((Test-SFTPPath -SessionId $sftp.SessionId -Path $destination/$leaf))
{
    Write-Output "Deleting old version..."
    Remove-SFTPItem -SessionId $sftp.SessionId -Path $destination/$leaf -Force
}

# Upload...
Write-Output "Uploading files..."
Set-SFTPItem -SessionId $sftp.SessionId -Destination $destination -Path $path -Force

# Disconnect from server...
Write-Output "Closing SFTP session..."
Remove-SFTPSession -SFTPSession $sftp | Out-Null

# Start process...
Write-Output "Starting process via tmux"
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "sudo chmod +x $destination/$leaf/VoidHuntersRevived.Server" | Out-Null
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "tmux new-session -d -s VoidHuntersRevived 'cd $destination/$leaf && ./VoidHuntersRevived.Server'" | Out-Null

Write-Output "Closing SSH session..."
Remove-SSHSession -SessionId $ssh.SessionId | Out-Null

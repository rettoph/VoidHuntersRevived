#Requires -Version 6.0

$InformationPreference = 'Continue'

$stopwatch =  [system.diagnostics.stopwatch]::StartNew()

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
    Write-Information "Loaded config from: $($configFile)"
}
else 
{
    $config = @{
        Remote = @{
            Host = "";
            Username = "";
            Password = "";
            Folder = "/home/anthony/VoidHuntersRevived";
            Launch = "VoidHuntersRevived.Server";
        };
        Build = @{
            Project = "Server";
            Configuration = "Debug";
            Runtime = "linux_arm64";
            SelfContained = 1;
        }
    }

    New-Item $configFile
    Set-Content $configFile ($config | ConvertTo-Json)

    Write-Information "No valid configuration file found. Please update the one at $($configPath)"

    Exit
}

# Build locally...
"$($stopwatch.Elapsed.ToString("m\:ss\.ff")): Building Project: '" + $config.Build.Project + "' - " + $config.Build.Runtime
$path = Publish-VoidHunters -project $config.Build.Project -configuration $config.Build.Configuration -runtime $config.Build.Runtime -selfContained $config.Build.SelfContained

# Connect to server...
"$($stopwatch.Elapsed.ToString("m\:ss\.ff")): Establishing connection with " + $config.Remote.Host
$password = ConvertTo-SecureString $config.Remote.Password -AsPlainText -Force
$credentials = New-Object System.Management.Automation.PSCredential ($config.Remote.Username, $password)
$sftp = New-SFTPSession -ComputerName $config.Remote.Host -Credential $credentials
$ssh = New-SSHSession -ComputerName $config.Remote.Host -Credential $credentials
$destination = $config.Remote.Folder

"$($stopwatch.Elapsed.ToString("m\:ss\.ff")): Killing any running tmux session..."
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "tmux kill-session -t VoidHuntersRevived" | Out-Null

# Ensure target folder exists...
if((Test-SFTPPath -SessionId $sftp.SessionId -Path $destination) -eq 0)
{
    "Creating $destination directory..."
    New-SFTPItem -SessionId $sftp.SessionId -Path $destination -ItemType Directory
}

# Upload dirty files
"$($stopwatch.Elapsed.ToString("m\:ss\.ff")): Preparing for upload..."
$files = Get-ChildItem $path -Recurse -Include "*" -Force

$cacheFile = $PSScriptRoot + "\remote-debug.cache.json"
[hashtable]$cache = Get-Content $cacheFile | ConvertFrom-Json -AsHashtable
[hashtable]$newCache = @{}
Write-Information "Loaded cache: $($cacheFile)"

function RemoteDestination($local)
{
    if($local -eq $null)
    {
        return $null
    }

    return $local.Replace("$path", "$destination").Replace("\", "/")
}
function CheckCacheDirty($target)
{
    $key = $target.path
    $hash = $cache[$key]
    
    if($hash -eq $null)
    {
        return $true
    }

    $dirty = $hash -ne $target.hash

    return $dirty
}
function CleanFile($file)
{
    try
    {
        $target = @{
            path = RemoteDestination($file.FullName);
        }

        if($file.GetType() -eq [System.IO.DirectoryInfo])
        {
            if((Test-SFTPPath -SessionId $sftp.SessionId -Path $target.path) -eq $true)
            {
                return $null
            }

            Write-Information "Creating folder: '$($target.path)'"
            New-SFTPItem -SessionId $sftp.SessionId -Path $target.path -ItemType Directory
            return $null
        }

        $target.hash = (Get-FileHash $file.FullName).Hash

        $dirty = CheckCacheDirty($target)

        if($dirty -eq 1)
        {
            $target.directory = RemoteDestination($file.DirectoryName)

            Write-Information "Uploading file: '$($target.path)'"
            Set-SFTPItem -SessionId $sftp.SessionId -Destination $target.directory -Path $file.FullName -Force
        }

        return $target
    }
    catch
    {
        Write-Error "An error occured uploading file: '$($file.FullName)'"
        Write-Error $_

        return $null
    }
}

"$($stopwatch.Elapsed.ToString("m\:ss\.ff")): Uploading files..."
foreach ($file in $files)
{
    $target = CleanFile($file)

    if($target -ne $null -and $target.hash -ne $null)
    {
        $newCache[$target.path] = $target.hash
    }
}

Set-Content $cacheFile (ConvertTo-Json $newCache)

# Start process...
"$($stopwatch.Elapsed.ToString("m\:ss\.ff")): Starting process via tmux..."
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "sudo chmod +x $destination/$($config.Remote.Launch)" | Out-Null
Invoke-SSHCommand -SessionId $ssh.SessionId -Command "tmux new-session -d -s VoidHuntersRevived 'cd $destination && ./$($config.Remote.Launch)'" | Out-Null

# Disconnect from server...
"$($stopwatch.Elapsed.ToString("m\:ss\.ff")): Closing connection..."
Remove-SSHSession -SessionId $ssh.SessionId | Out-Null
Remove-SFTPSession -SFTPSession $sftp | Out-Null

"Done. Total Elapsed Time: " + $stopwatch.Elapsed.ToString("m\:ss\.ff")
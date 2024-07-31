function Get-Directory(
    [string] $path,
    [bool] $createIfNotFound = $false
)
{
    $output = [IO.Path]::GetFullPath($path);

    if([IO.Directory]::Exists($output) -eq $false)
    {
        if($createIfNotFound -eq $false)
        {
            Write-Error "Error: Invalid directory '$output'"
            exit
        }

        [IO.Directory]::CreateDirectory($output);
    }

    return $output
}

function Get-File(
    [string] $path,
    [bool] $createIfNotFound = $false
)
{
    $output = [IO.Path]::GetFullPath($path);

    if([IO.File]::Exists($output) -eq $false)
    {
        if($createIfNotFound -eq $false)
        {
            Write-Error "Error: Invalid file '$output'"
            exit
        }

        # Ensure file directory exists
        $directory = [IO.Path]::GetDirectoryName($output)
        $directory = Get-Directory $directory $true

        [IO.File]::Create($output).Close()
    }

    return $output
}

function Get-StringHash($value)
{
    $stream = [IO.MemoryStream]::new([byte[]][char[]]$value)
    $result = Get-FileHash -InputStream $stream -Algorithm SHA256
    $stream.Close()

    return $result.Hash
}
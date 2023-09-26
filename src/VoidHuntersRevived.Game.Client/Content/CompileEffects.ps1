#Requires -Version 6.0

$InformationPreference = 'Continue'

$cacheFile = $PSScriptRoot + "\.fx.cache.json"
$fxPath = $PSScriptRoot + "\Client\Effects\"
$mgfxPath = $PSScriptRoot + "\Client\Effects\Compiled\"
[hashtable]$cache = Get-Content $cacheFile | ConvertFrom-Json -AsHashtable
[hashtable]$newCache = @{}
Write-Information "Loaded cache: $($cacheFile)"

$files = Get-ChildItem $fxPath -Filter "*.fx"

function CheckCacheDirty($file, $newHash)
{
    if($null -eq $cache)
    {
        return $true
    }

    if($cache.ContainsKey($file) -eq $false)
    {
        return $true
    }

    $oldHash = $cache[$file]

    $dirty = $newHash -ne $oldHash

    return $dirty
}

foreach ($file in $files)
{
    $hash = (Get-FileHash $file).Hash
    $dirty = CheckCacheDirty $file.Name $hash

    if($dirty -eq $true)
    {
        $compiledPath = $mgfxPath + $file.BaseName + ".mgfx"
        $allOutput = & mgfxc $file.FullName $compiledPath /Profile:OpenGL 2>&1
        $stderr = $allOutput | Where-Object { $_ -is [System.Management.Automation.ErrorRecord] }
        if($null -ne $stderr)
        {
            "error $($stderr.Exception)"
        }
        else
        {
            $newCache[$file.Name] = $hash
        }
    }
}

Set-Content $cacheFile (ConvertTo-Json $newCache)

exit 0
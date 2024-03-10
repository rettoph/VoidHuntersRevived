#Requires -Version 6.0

$InformationPreference = 'Continue'


$cacheFile = $PSScriptRoot + "\.fx.cache.json"
$fxPath = $PSScriptRoot + "\Client\Effects\"
$mgfxPath = $PSScriptRoot + "\Client\Effects\Compiled\"
$mgfxcPath = $PSScriptRoot + "\..\..\..\..\libraries\Guppy\libraries\MonoGame\Artifacts\MonoGame.Effect.Compiler\Release\mgfxc.exe"
[hashtable]$cache = Get-Content $cacheFile | ConvertFrom-Json -AsHashtable
[hashtable]$newCache = @{}
Write-Information "Loaded cache: $($cacheFile)"

$mgfxcExists = Test-Path -Path $mgfxcPath -PathType Leaf
if($mgfxcExists -eq $false)
{
    Write-Information "Publishing mgfxc.exe"
    $mgfxcProj = $PSScriptRoot + "\..\..\..\..\libraries\Guppy\libraries\MonoGame\Tools\MonoGame.Effect.Compiler\MonoGame.Effect.Compiler.csproj"
    dotnet publish $mgfxcProj -c Release
}

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

    return $true
}

foreach ($file in $files)
{
    $hash = (Get-FileHash $file).Hash
    $dirty = CheckCacheDirty $file.Name $hash

    if($dirty -eq $true)
    {
        $compiledPath = $mgfxPath + $file.BaseName + ".mgfx"
        $allOutput = & $mgfxcPath $file.FullName $compiledPath /Profile:OpenGL 2>&1
        $stderr = $allOutput | Where-Object { $_ -is [System.Management.Automation.ErrorRecord] }
        if($null -ne $stderr)
        {
            "error $($stderr.Exception)"
            exit 1
        }
        else
        {
            $newCache[$file.Name] = $hash
        }
    }
    else {
        $newCache[$file.Name] = $hash
    }
}

Set-Content $cacheFile (ConvertTo-Json $newCache)

exit 0
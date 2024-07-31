Import-Module -Name ($PSScriptRoot + "\modules\utilities-module.ps1") -Force

$InformationPreference = 'Continue'

$inputDirectory = Get-Directory $args[0]
$outputDirectory = Get-Directory $args[1] $true

$argsHash = Get-StringHash ($inputDirectory + $outputDirectory)
$cacheFile = Get-File "$PSScriptRoot\cache\.compile-fx.cache.$argsHash.json" $true
[hashtable]$cache = Get-Content $cacheFile | ConvertFrom-Json -AsHashtable
[hashtable]$newCache = @{}
Write-Information "Loaded cache: '$($cacheFile)'"

$mgfxcPath = Get-File ($PSScriptRoot + "\..\libraries\Guppy\libraries\MonoGame\Artifacts\MonoGame.Effect.Compiler\Release\mgfxc.exe")

Write-Output $mgfxcPath

if([IO.Path]::Exists($mgfxcPath) -eq $false)
{
    Write-Information "Publishing mgfxc.exe"
    $mgfxcProj = [IO.Path]::GetFullPath($PSScriptRoot + "\..\libraries\Guppy\libraries\MonoGame\Tools\MonoGame.Effect.Compiler\MonoGame.Effect.Compiler.csproj")
    dotnet publish $mgfxcProj -c Release
}

function Get-EffectHash($fxPath)
{
    $hash = (Get-FileHash $fxPath).Hash

    $content = Get-Content $fxPath
    $result = [regex]::Match($content, '#include "(.*\.fx)\"')

    if($result.Success -eq $false)
    {
        return $hash
    }

    $fxDirectory = [IO.Path]::GetDirectoryName($fxPath)

    foreach($capture in $result.Captures)
    {
        $includePath = Get-File "$fxDirectory\$($capture.Groups[1])"
        $hash += (Get-FileHash $includePath).Hash
    }

    return $hash
}

function Get-IsEffectDirty($file, $newHash)
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

$cleaned = 0
$files = Get-ChildItem $inputDirectory -Filter "*.fx"
foreach ($file in $files)
{
    if($file.BaseName.StartsWith("_") -eq $true)
    { # Exclude partial files (file names with _ prefix)
        continue
    }

    $hash = Get-EffectHash $file.FullName | Write-Output
    $dirty = Get-IsEffectDirty $file.Name $hash

    if($dirty -eq $true)
    {
        Write-Information "Updating: $($file.Name)"

        $compiledPath = "$outputDirectory\$($file.BaseName).mgfx"
        $allOutput = & $mgfxcPath $file.FullName $compiledPath /Profile:OpenGL 2>&1
        $stderr = $allOutput | Where-Object { $_ -is [System.Management.Automation.ErrorRecord] }
        $cleaned++

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
        Write-Information "Skipping: $($file.Name)"

        $newCache[$file.Name] = $hash
    }
}

if($cleaned -gt 0)
{
    Set-Content $cacheFile (ConvertTo-Json $newCache)
}

exit 0
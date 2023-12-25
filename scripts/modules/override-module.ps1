Import-Module -Name ($PSScriptRoot + "\source-module.ps1") -Force

class VoidHuntersOverride
{
    [hashtable]$originals

    VoidHuntersOverride()
    {
        $this.originals = @{}
    }

    [object[]] GetFiles()
    {
        return Get-ChildItem $global:SourcePath -Recurse -Include "*.cs"
    }

    [string] GetContent([object]$file)
    {
        return Get-Content -Path $file.FullName -Raw
    }

    [void] SetContent([object]$file, [string]$content)
    {
        if($this.originals.ContainsKey($file.FullName) -eq $false)
        {
            $this.originals.Add($file.FullName, $this.GetContent($file))
        }

        Set-Content -Path $file.FullName -Value ([byte[]][char[]]$content) -AsByteStream
    }

    [void] RemoveAll([string]$regex)
    {
        foreach($file in $this.GetFiles())
        {
            $content = $this.GetContent($file)
            $contentMatches = [regex]::Matches($content, $regex, [Text.RegularExpressions.RegexOptions]'Multiline')
            
            if($contentMatches.Count -eq 0)
            {
                continue
            }
            
            $offset = 0
            foreach($match in $contentMatches)
            {
                $content = $content.Substring(0, $match.Index - $offset) + $content.Substring($match.Index + $match.Length - $offset)
                $offset += $match.Length
            }

            $this.SetContent($file, $content)
        }
    }

    [void] Reset()
    {
        foreach($kvp in $this.originals)
        {
            Set-Content -Path $kvp.key -Value ([byte[]][char[]]$kvp.value) -AsByteStream
        }
    }
}


$override = [VoidHuntersOverride]::new()

$override.RemoveAll('^( |\t)*?.*?(l|L)ogger\.Verbose\(.*?\);\s*$')


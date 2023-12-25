Import-Module -Name ($PSScriptRoot + "\source-module.ps1") -Force

class VoidHuntersEditor
{
    [hashtable]$originals

    VoidHuntersEditor()
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

        [IO.File]::WriteAllText($file.FullName, $content, [Text.Encoding]::UTF8)
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
        foreach($kvp in $this.originals.GetEnumerator())
        {
            [IO.File]::WriteAllText($kvp.key, $kvp.value, [Text.Encoding]::UTF8)
        }
    }
}


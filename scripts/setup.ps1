Set-Location $PSScriptRoot;

git submodule update --init --recursive

./../libraries/Guppy/scripts/install.ps1

<#
 #
 # The following code might actually not be needed 
 # 'nuget.config' might be automatically imported via
 # file hierarchy
 #

$SolutionDirectory = "$($PSScriptRoot)/.." | Resolve-Path
$GuppyNugetDirectory = "$($SolutionDirectory)/libraries/Guppy/.nuget" | Resolve-Path

@'
<!-- Generated via VoidHuntersRevived/scripts/install.ps1 -->
<configuration>
  <packageSources>
    <add key="GuppyPackages" value="{0}" />
  </packageSources>
</configuration>
'@ -f $GuppyNugetDirectory | Out-File -FilePath "$($SolutionDirectory)/nuget.config"
#>
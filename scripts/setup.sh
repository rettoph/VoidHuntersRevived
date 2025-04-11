#!/usr/bin/env bash

# Move to the script's directory
cd "$(dirname "${BASH_SOURCE[0]}")" || exit 1

# Update git submodules
git submodule update --init --recursive

# Run Guppy's install script (assuming it can be run by PowerShell on the system)
./../libraries/Guppy/scripts/install.sh

: <<'COMMENT'
 #
 # The following code might actually not be needed 
 # 'nuget.config' might be automatically imported via
 # file hierarchy
 #
COMMENT

SolutionDirectory="$(realpath "$(dirname "${BASH_SOURCE[0]}")/..")"
GuppyNugetDirectory="$(realpath "$SolutionDirectory/libraries/Guppy/.nuget")"

cat <<EOF > "$SolutionDirectory/nuget.config"
<!-- Generated via VoidHuntersRevived/scripts/install.sh -->
<configuration>
  <packageSources>
    <add key="GuppyPackages" value="$GuppyNugetDirectory" />
  </packageSources>
</configuration>
EOF

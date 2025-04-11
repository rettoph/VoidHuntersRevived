#!/bin/bash
set -e

# === CONFIGURATION ===
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MGFXC_PATH="$SCRIPT_DIR/../libraries/Guppy/libraries/MonoGame/Artifacts/MonoGame.Effect.Compiler/Release/mgfxc"

# === ARGUMENTS ===
INPUT_DIR="$1"
OUTPUT_DIR="$2"

if [[ -z "$INPUT_DIR" || -z "$OUTPUT_DIR" ]]; then
    echo "Usage: $0 <input_directory> <output_directory>"
    exit 1
fi

# === Ensure output directory exists ===
mkdir -p "$OUTPUT_DIR"

# === Compile Function ===
function compile_fx() {
    local input_path="$1"
    local base_name
    base_name="$(basename "$input_path" .fx)"
    local output_path="$OUTPUT_DIR/$base_name.mgfx"

    echo "Compiling $input_path -> $output_path"
    "$MGFXC_PATH" "$input_path" "$output_path" /Profile:OpenGL
}

# === Main ===
find "$INPUT_DIR" -maxdepth 1 -type f -name "*.fx" | while read -r fx_file; do
    filename="$(basename "$fx_file")"
    if [[ "$filename" == _* ]]; then
        echo "Skipping partial: $filename"
        continue
    fi

    compile_fx "$fx_file"
done

echo "✅ Compilation complete."
$MonoGameDirectory = "./../libraries/Guppy/libraries/MonoGame";
$MonoGameContentBuilderDirectory = $MonoGameDirectory + "/Tools/MonoGame.Content.Builder"
$MonoGameContentBuilderEditorWindowsDirectory = $MonoGameDirectory + "/Tools/MonoGame.Content.Builder.Editor/MonoGame.Content.Builder.Editor.Windows.csproj"
$MonoGameContentBuilderEditorWindowsLauncherDirectory = $MonoGameDirectory + "/Tools/MonoGame.Content.Builder.Editor.Launcher\MonoGame.Content.Builder.Editor.Launcher.Windows.csproj"
$MonoGameContentBuilderEditorBootstrapLauncherDirectory = $MonoGameDirectory + "/Tools/MonoGame.Content.Builder.Editor.Launcher.Bootstrap"

dotnet publish -c Release $MonoGameContentBuilderEditorWindowsDirectory

# Pack & Install dotnet-mgcb for Content.mgcb building
# https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create
# https://learn.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use
dotnet pack -o ./bin $MonoGameContentBuilderDirectory
dotnet tool install --version 1.0.0 --add-source ./bin dotnet-mgcb

dotnet pack -o ./bin $MonoGameContentBuilderEditorWindowsLauncherDirectory
dotnet tool install --version 1.0.0 --add-source ./bin dotnet-mgcb-editor-windows

dotnet pack -o ./bin $MonoGameContentBuilderEditorBootstrapLauncherDirectory
dotnet tool install --version 1.0.0 --add-source ./bin dotnet-mgcb-editor

Remove-Item -Recurse -Force ./bin

# dotnet tool uninstall dotnet-mgcb
# dotnet tool uninstall dotnet-mgcb-editor-windows
# dotnet tool uninstall dotnet-mgcb-editor
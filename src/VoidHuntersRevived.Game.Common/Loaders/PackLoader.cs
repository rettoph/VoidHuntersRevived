using Guppy.Attributes;
using Guppy.Files.Enums;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;

namespace VoidHuntersRevived.Game.Common.Loaders
{
    [AutoLoad]
    internal sealed class PackLoader : IPackLoader
    {
        public void Load(IResourcePackProvider packs)
        {
            packs.Register(FileType.CurrentDirectory, Path.Combine(VoidHuntersPack.Directory, "pack.json"));
        }
    }
}

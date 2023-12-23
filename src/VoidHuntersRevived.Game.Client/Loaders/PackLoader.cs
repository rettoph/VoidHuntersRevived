using Guppy.Attributes;
using Guppy.Files.Enums;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Content;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class PackLoader : IPackLoader
    {
        private readonly ContentManager _content;

        public PackLoader(ContentManager content)
        {
            _content = content;
        }

        public void Load(IResourcePackProvider packs)
        {
            _content.RootDirectory = VoidHuntersPack.Directory;

            packs.Register(FileType.CurrentDirectory, Path.Combine(VoidHuntersPack.Directory, "pack.json"));
        }
    }
}

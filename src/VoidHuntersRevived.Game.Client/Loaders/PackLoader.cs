using Guppy.Attributes;
using Guppy.Files.Enums;
using Guppy.Files.Services;
using Guppy.Resources.Constants;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Game.Common;

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

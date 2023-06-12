using Guppy.Attributes;
using Guppy.MonoGame.Resources;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Game.Client.Constants;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class PackLoader : IPackLoader
    {
        public void Load(IPackProvider packs)
        {
            packs.GetById(VoidHuntersPack.Id)
                .Add(new SpriteFontResource(Fonts.Default, "Fonts/BiomeLight-Normal"));
        }
    }
}

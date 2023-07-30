using Guppy.Attributes;
using Guppy.Resources;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Common.Loaders
{
    [AutoLoad]
    internal class ResourcePackLoader : IPackLoader
    {
        public void Load(IResourcePackProvider packs)
        {
            packs.Configure(VoidHuntersPack.Id, pack =>
            {
                pack.Add(Resources.Colors.Orange, new Color(Color.Orange, 0.85f));
            });
        }
    }
}

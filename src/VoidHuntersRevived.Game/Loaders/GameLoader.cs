using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame.Resources;
using Guppy.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal sealed class GameLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Pack>(new Pack(VoidHuntersPack.Id, VoidHuntersPack.Name)
            {
                Directory = VoidHuntersPack.Directory
            }.Add(new ColorResource(Colors.Orange, Color.Orange)));
        }
    }
}

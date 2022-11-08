using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library.Systems;

namespace VoidHuntersRevived.Client.Library.Loaders
{
    [AutoLoad]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<ClientMainGuppy>();
            services.AddGuppy<ClientGameGuppy>();

            services.AddScoped<Camera2D>();

            services.AddSystem<AetherDebugSystem>();
        }
    }
}

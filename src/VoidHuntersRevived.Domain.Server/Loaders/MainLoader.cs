using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Simulations.Systems;
using Guppy.Common.DependencyInjection;

namespace VoidHuntersRevived.Domain.Server.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<ServerGameGuppy>();

            services.ConfigureCollection(manager =>
            {
 
            });
        }
    }
}

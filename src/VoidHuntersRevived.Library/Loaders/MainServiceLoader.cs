using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Network.Enums;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Systems;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITickService, TickService>()
                    .AddScoped<ITickFactory, TickFactory>()
                    .AddScoped<TickBuffer>();

            services.AddSystem<MasterTickSystem>(0)
                    .AddSystem<SlaveTickSystem>(0);

            services.AddScoped<LocalTickProvider>()
                    .AddScoped<RemoteTickProvider>()
                    .AddFilter<ITickProvider, LocalTickProvider>(NetAuthorizationFilter(NetAuthorization.Master), 0)
                    .AddFilter<ITickProvider, RemoteTickProvider>(NetAuthorizationFilter(NetAuthorization.Slave), 0);
        }

        public Func<IServiceProvider, bool> NetAuthorizationFilter(NetAuthorization authorization)
        {
            bool Filter(IServiceProvider provider)
            {
                return provider.GetSetting<NetAuthorization>().Value == authorization;
            }

            return Filter;
        }
    }
}

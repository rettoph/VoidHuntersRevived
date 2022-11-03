﻿using Guppy.Attributes;
using Guppy.Common;
using Guppy.Loaders;
using Guppy.Network.Enums;
using Guppy.Resources.Filters;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            services.AddScoped<LocalTickProvider>()
                    .AddScoped<RemoteTickProvider>()
                    .AddAliases(Alias.ForMany<ITickProvider>(typeof(LocalTickProvider), typeof(RemoteTickProvider)))
                    .AddFilter(new SettingFilter<NetAuthorization, LocalTickProvider>(NetAuthorization.Master))
                    .AddFilter(new SettingFilter<NetAuthorization, RemoteTickProvider>(NetAuthorization.Slave));
        }
    }
}

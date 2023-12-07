using Autofac;
using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Autofac;
using Guppy.Common.Extensions.Autofac;
using Guppy.Extensions.Autofac;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Providers;
using Guppy.Game;
using Guppy.Game.Common;
using Guppy.Game.Extensions.Serilog;
using Guppy.Loaders;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Client.Services;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    public sealed class ClientLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<VisibleRenderingService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}

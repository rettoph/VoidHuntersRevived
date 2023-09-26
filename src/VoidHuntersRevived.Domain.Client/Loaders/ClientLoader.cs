using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
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

using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Services;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal sealed class ServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<TractorBeamEmitterService>().AsSelf().As<IEngine>().InstancePerLifetimeScope();
        }
    }
}

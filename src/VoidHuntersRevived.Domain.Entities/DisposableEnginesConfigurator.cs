using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Configurations;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities
{
    [AutoLoad]
    internal sealed class DisposableEnginesConfigurator : IGuppyConfigurator
    {
        public void Configure(GuppyConfiguration configuration)
        {
            foreach (Type disposableComponent in configuration.Assemblies.GetTypes<IEntityComponent>().Where(x => x.IsAssignableTo<IDisposable>()))
            {
                configuration.Builder.RegisterType(typeof(DisposableEngine<>).MakeGenericType(disposableComponent))
                    .As<IEngine>()
                    .InstancePerLifetimeScope();
            }
        }
    }
}

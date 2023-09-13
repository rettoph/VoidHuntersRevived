using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Configurations;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Attributes;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Configurators
{
    [AutoLoad]
    internal sealed class DisposableEnginesConfigurator : IGuppyConfigurator
    {
        public void Configure(GuppyConfiguration configuration)
        {
            foreach (Type disposableComponent in configuration.Assemblies.GetTypes<IEntityComponent>().Where(this.ShouldAutoDisposeComponentPerInstance))
            {
                configuration.Builder.RegisterType(typeof(InstanceDisposableEngine<>).MakeGenericType(disposableComponent))
                    .As<IEngine>()
                    .InstancePerLifetimeScope();
            }
        }

        private bool ShouldAutoDisposeComponentPerInstance(Type type)
        {
            if (type.IsAssignableTo<IDisposable>() == false)
            {
                return false;
            }

            AutoDisposeAttribute? autoDisposeAttr = type.GetCustomAttribute<AutoDisposeAttribute>(true);

            return (autoDisposeAttr?.Scope ?? AutoDisposeScope.None) == AutoDisposeScope.Instance;
        }
    }
}

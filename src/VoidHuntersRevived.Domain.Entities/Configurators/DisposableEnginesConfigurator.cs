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
            foreach (Type disposableComponent in configuration.Assemblies.GetTypes<IEntityComponent>())
            {
                foreach(AutoDisposeComponentAttribute autoDisposeAttr in disposableComponent.GetCustomAttributes<AutoDisposeComponentAttribute>(true))
                {
                    if(autoDisposeAttr.Scope == AutoDisposeScope.Instance)
                    {
                        configuration.Builder.RegisterType(typeof(InstanceDisposableEngine<>)
                            .MakeGenericType(autoDisposeAttr.GetDisposableComponentType(disposableComponent)))
                            .As<IEngine>()
                            .InstancePerLifetimeScope();
                    }
                }

            }
        }
    }
}

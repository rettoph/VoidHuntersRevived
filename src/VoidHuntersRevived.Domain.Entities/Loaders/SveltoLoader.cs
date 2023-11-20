using Autofac;
using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Attributes;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal class SveltoLoader : IServiceLoader
    {
        private readonly IAssemblyProvider _assemblies;

        public SveltoLoader(IAssemblyProvider assemblies)
        {
            _assemblies = assemblies;
        }

        public void ConfigureServices(ContainerBuilder services)
        {
            // Auto register an engine to dispose of instances as needed
            foreach (Type disposableComponent in _assemblies.GetTypes<IEntityComponent>())
            {
                foreach (AutoDisposeComponentAttribute autoDisposeAttr in disposableComponent.GetCustomAttributes<AutoDisposeComponentAttribute>(true))
                {
                    if (autoDisposeAttr.Scope == AutoDisposeScope.Instance)
                    {
                        services.RegisterType(typeof(InstanceDisposableEngine<>)
                            .MakeGenericType(autoDisposeAttr.GetDisposableComponentType(disposableComponent)))
                            .As<IEngine>()
                            .InstancePerLifetimeScope();
                    }
                }
            }

            // Register all Entity Descriptors
            var descriptors = _assemblies.GetTypes<VoidHuntersEntityDescriptor>().Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType).ToArray();
            foreach (Type descriptorType in descriptors)
            {
                services.RegisterType(descriptorType).As<VoidHuntersEntityDescriptor>().SingleInstance();
                services.RegisterType(typeof(VoidHuntersEntityDescriptorEngine<>).MakeGenericType(descriptorType)).AsImplementedInterfaces().InstancePerLifetimeScope();
            }
        }
    }
}

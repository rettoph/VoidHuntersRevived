using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Services;
using Guppy.Engine.Common.Loaders;
using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Domain.Entities.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal class SveltoLoader : IServiceLoader
    {
        private readonly IAssemblyService _assemblies;

        public SveltoLoader(IAssemblyService assemblies)
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
                        services.RegisterType(typeof(DisposableEngine<>)
                            .MakeGenericType(autoDisposeAttr.GetDisposableComponentType(disposableComponent)))
                            .As<IEngine>()
                            .InstancePerLifetimeScope();
                    }
                }
            }
        }
    }
}

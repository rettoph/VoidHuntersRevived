using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Services;
using Guppy.Engine.Common.Loaders;
using Svelto.ECS;
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
                if (disposableComponent.IsAssignableTo<IDisposable>())
                {
                    services.RegisterType(typeof(DisposableEngine<>).MakeGenericType(disposableComponent))
                        .As<IEngine>()
                        .InstancePerLifetimeScope();
                }
            }
        }
    }
}

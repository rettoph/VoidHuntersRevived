using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Services;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Modules
{
    [AutoLoad]
    internal class SveltoModule(IAssemblyService assemblies) : Module
    {
        private readonly IAssemblyService _assemblies = assemblies;

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Auto register an engine to dispose of instances as needed
            foreach (Type disposableComponent in _assemblies.GetTypes<IEntityComponent>())
            {
                if (disposableComponent.IsAssignableTo<IDisposable>())
                {
                    builder.RegisterType(typeof(DisposableEngine<>).MakeGenericType(disposableComponent))
                        .As<IEngine>()
                        .InstancePerLifetimeScope();
                }
            }
        }
    }
}

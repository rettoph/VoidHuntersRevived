using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Ships.Engines;
using VoidHuntersRevived.Domain.Ships.Serialization.Components;
using VoidHuntersRevived.Domain.Ships.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Ships.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Ships.Loaders
{
    [AutoLoad]
    public sealed class ShipLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<TractorBeamEmitterService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<TacticalService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UserShipService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterEngine<HelmEngine>();
            builder.RegisterEngine<TacticalEngine>();
            builder.RegisterEngine<TractorBeamEmitterInputEngine>();
            builder.RegisterEngine<TractorBeamEmitterUpdateEngine>();

            builder.RegisterNetSerializer<Helm_SetDirection_NetSerializer>();
            builder.RegisterNetSerializer<Input_TractorBeamEmitter_Deselect_NetSerializer>();
            builder.RegisterNetSerializer<Input_TractorBeamEmitter_Select_NetSerializer>();
            builder.RegisterNetSerializer<Tactical_SetTarget_NetSerialization>();

            builder.RegisterComponentSerializer<HelmComponentSerializer>();
            builder.RegisterComponentSerializer<TacticalComponentSerializer>();
            builder.RegisterComponentSerializer<TractorableComponentSerializer>();
            builder.RegisterComponentSerializer<TractorBeamEmitterComponentSerializer>();
            builder.RegisterComponentSerializer<UserIdComponentSerializer>();
        }
    }
}

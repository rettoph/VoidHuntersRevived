using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Ships.Serialization.Components;
using VoidHuntersRevived.Domain.Ships.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Ships.Services;
using VoidHuntersRevived.Domain.Ships.Systems;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterDomainShipsServices(this IGuppyScopeBuilder builder)
        {
            builder.RegisterNetSerializer<Helm_SetDirection_NetSerializer>();
            builder.RegisterNetSerializer<Input_TractorBeamEmitter_Deselect_NetSerializer>();
            builder.RegisterNetSerializer<Input_TractorBeamEmitter_Select_NetSerializer>();
            builder.RegisterNetSerializer<Tactical_SetTarget_NetSerialization>();

            return builder.EnsureRegisteredOnce(nameof(RegisterDomainShipsServices), builder =>
            {
                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterType<TractorBeamEmitterService>().As<ITractorBeamEmitterService>().InstancePerLifetimeScope();
                    builder.RegisterType<TacticalService>().AsImplementedInterfaces().InstancePerLifetimeScope();
                    builder.RegisterType<UserIdSystem>().AsImplementedInterfaces().InstancePerLifetimeScope();

                    builder.RegisterSceneSystem<HelmSystem>();
                    builder.RegisterSceneSystem<TacticalSystem>();
                    builder.RegisterSceneSystem<TractorBeamEmitterSelectAndDeselectEventSystem>();
                    builder.RegisterSceneSystem<TractorBeamEmitterInputSystem>();
                    builder.RegisterSceneSystem<TractorBeamEmitterUpdateSystem>();

                    builder.RegisterComponentSerializer<HelmComponentSerializer>();
                    builder.RegisterComponentSerializer<TacticalComponentSerializer>();
                    builder.RegisterComponentSerializer<TractorableComponentSerializer>();
                    builder.RegisterComponentSerializer<TractorBeamEmitterComponentSerializer>();
                    builder.RegisterComponentSerializer<UserIdComponentSerializer>();
                });
            });
        }
    }
}
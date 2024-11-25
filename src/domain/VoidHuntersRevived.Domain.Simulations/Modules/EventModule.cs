using Autofac;
using Guppy.Core.Common.Attributes;
using LiteNetLib;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers;

namespace VoidHuntersRevived.Domain.Simulations.Modules
{
    [AutoLoad]
    internal sealed class EventModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterNetMessageType<EventDto>(DeliveryMethod.ReliableUnordered, 0);

            builder.RegisterNetSerializer<EventDtoNetSerializer>();
            builder.RegisterNetSerializer<Simulation_Begin_NetSerializer>();
            builder.RegisterNetSerializer<UserJoinedNetSerializer>();
        }
    }
}

using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using LiteNetLib;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers;

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    [AutoLoad]
    internal sealed class EventLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterNetMessageType<EventDto>(DeliveryMethod.ReliableUnordered, 0);

            services.RegisterNetSerializer<EventDtoNetSerializer>();
            services.RegisterNetSerializer<Simulation_Begin_NetSerializer>();
            services.RegisterNetSerializer<UserJoinedNetSerializer>();
        }
    }
}

using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using LiteNetLib;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    [AutoLoad]
    internal sealed class EventLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.AddNetMessageType<EventDto>(DeliveryMethod.ReliableUnordered, 0);
        }
    }
}

using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using LiteNetLib;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Loaders
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

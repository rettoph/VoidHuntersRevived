using Guppy.Attributes;
using Guppy.Loaders;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal sealed class EventLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNetMessageType<EventDto>(DeliveryMethod.ReliableUnordered, 0);
        }
    }
}

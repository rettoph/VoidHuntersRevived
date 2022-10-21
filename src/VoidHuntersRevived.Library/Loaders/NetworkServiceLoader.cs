using Guppy.Attributes;
using Guppy.Loaders;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Models;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class NetworkServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, NetMessageTypeConstants.DefaultOutgoingChannel);
            services.AddNetMessageType<GameState>(DeliveryMethod.ReliableOrdered, NetMessageTypeConstants.DefaultOutgoingChannel);
        }
    }
}

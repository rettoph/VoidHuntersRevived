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
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad(0)]
    internal sealed class NetworkServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNetMessageType<DirectionInput>(DeliveryMethod.ReliableSequenced, NetMessageTypeConstants.DefaultOutgoingChannel);
            services.AddNetMessageType<SimulationStateTick>(DeliveryMethod.ReliableOrdered, NetMessageTypeConstants.GameStateOutgoingChannel);
            services.AddNetMessageType<SimulationStateEnd>(DeliveryMethod.ReliableOrdered, NetMessageTypeConstants.GameStateOutgoingChannel);
            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, NetMessageTypeConstants.TickOutgoingChannel);
        }
    }
}

using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Providers;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal partial class SimulationEventPublishingService
    {
        private interface ISimulationEventPublisher
        {
            ISimulationEvent Publish(ISimulation simulation, SimulationEventData core);
        }

        private class SimulationEventPublisher<T> : ISimulationEventPublisher
            where T : class
        {
            private readonly IBus _bus;

            public SimulationEventPublisher(IBus bus)
            {
                _bus = bus;
            }

            public ISimulationEvent Publish(ISimulation simulation, SimulationEventData data)
            {
                SimulationEvent<T> message = new SimulationEvent<T>()
                {
                    Key = data.Key,
                    SenderId = data.SenderId,
                    Simulation = simulation,
                    Body = Unsafe.As<T>(data.Body)
                };

                _bus.Publish(message);

                return message;
            }
        }
    }
}

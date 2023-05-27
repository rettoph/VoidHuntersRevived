using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal partial class SimulationEventPublishingService
    {
        private interface ISimulationEventPublisher
        {
            ISimulationEvent Publish(ISimulation simulation, SimulationEventData data);
            ISimulationEventRevision Revert(ISimulationEvent simulationEvent);
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

            public ISimulationEventRevision Revert(ISimulationEvent simulationEvent)
            {
                SimulationEventRevision<T> message = new SimulationEventRevision<T>()
                {
                    Key = simulationEvent.Key,
                    SenderId = simulationEvent.SenderId,
                    Simulation = simulationEvent.Simulation,
                    Body = Unsafe.As<T>(simulationEvent.Body),
                    Response = simulationEvent.Response
                };

                _bus.Publish(message);

                return message;
            }
        }
    }
}

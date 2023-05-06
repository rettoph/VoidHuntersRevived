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
            private readonly ISimulationEventListener<T>[] _subscribers;

            public SimulationEventPublisher(IEnumerable<object> subscribers)
            {
                _subscribers = subscribers.OfType<ISimulationEventListener<T>>().ToArray();
            }

            public ISimulationEvent Publish(ISimulation simulation, SimulationEventData core)
            {
                SimulationEvent<T> @event = new SimulationEvent<T>()
                {
                    Key = core.Key,
                    SenderId = core.SenderId,
                    Simulation = simulation,
                    Body = Unsafe.As<T>(core.Body)
                };

                foreach (ISimulationEventListener<T> subscriber in _subscribers)
                {
                    subscriber.Process(@event);
                }

                return @event;
            }
        }
    }
}

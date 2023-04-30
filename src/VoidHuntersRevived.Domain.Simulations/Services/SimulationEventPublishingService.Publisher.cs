using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal partial class SimulationEventPublishingService
    {
        private interface ISimulationEventPublisher
        {
            SimulationEventResult Publish(ISimulation simulation, ISimulationEventData data);
        }

        private class SimulationEventPublisher<T> : ISimulationEventPublisher
            where T : class, ISimulationEventData
        {
            private readonly ISimulationEventListener<T>[] _subscribers;

            public SimulationEventPublisher(IEnumerable<object> subscribers)
            {
                _subscribers = subscribers.OfType<ISimulationEventListener<T>>().ToArray();
            }

            public SimulationEventResult Publish(ISimulation simulation, ISimulationEventData data)
            {
                T casted = Unsafe.As<T>(data);
                SimulationEventResult result = SimulationEventResult.Success;

                foreach (ISimulationEventListener<T> subscriber in _subscribers)
                {
                    result |= subscriber.Process(simulation, casted);
                }

                return result;
            }
        }
    }
}

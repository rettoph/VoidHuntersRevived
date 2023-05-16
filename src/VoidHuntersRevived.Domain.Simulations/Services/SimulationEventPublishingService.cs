using Guppy.Common;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class SimulationEventPublishingService : ISimulationEventPublishingService
    {
        private readonly Dictionary<Type, ISimulationEventPublisher> _subscribers;
        private readonly IBus _bus;

        public SimulationEventPublishingService(IBus bus)
        {
            _bus = bus;
            _subscribers = new Dictionary<Type, ISimulationEventPublisher>();
        }

        public ISimulationEvent Publish(ISimulation simulation, SimulationEventData core)
        {
            return this.GetInputPublisher(core.Body.GetType()).Publish(simulation, core);
        }

        private ISimulationEventPublisher GetInputPublisher(Type inputType)
        {
            ref ISimulationEventPublisher? publisher = ref CollectionsMarshal.GetValueRefOrAddDefault(_subscribers, inputType, out bool exists);

            if (!exists)
            {
                Type publisherType = typeof(SimulationEventPublisher<>).MakeGenericType(inputType);
                publisher = (ISimulationEventPublisher)Activator.CreateInstance(publisherType, new object[] { _bus })!;
            }

            return publisher!;
        }
    }
}

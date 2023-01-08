using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Library.Simulations.Services
{
    internal sealed class GlobalSimulationService : IGlobalSimulationService
    {
        private readonly IList<ISimulation> _items;
        public ReadOnlyCollection<ISimulation> Instances { get; private set; }

        public GlobalSimulationService()
        {
            _items = new List<ISimulation>();

            this.Instances = new ReadOnlyCollection<ISimulation>(_items);
        }

        public void Add(ISimulation instance)
        {
            _items.Add(instance);
        }

        public void Remove(ISimulation instance)
        {
            _items.Remove(instance);
        }
    }
}

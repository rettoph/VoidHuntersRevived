using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IParallelEntityService
    {
        event Action<ParallelKey, ISimulation>? EntityAdded;
        event Action<ParallelKey, ISimulation>? EntityRemoved;
        event Action<ParallelKey, ISimulation, BitVector32>? EntityChanged;

        IEnumerable<ParallelKey> Entities { get; }

        int GetEntityId(ParallelKey entityKey, ISimulation simulation);
        ParallelKey GetEntityKey(int entityId);

        void Map(int entityId, ParallelKey key, ISimulation simulation);
        void Unmap(ParallelKey key, ISimulation simulation, out int entityId);

        bool TryGetComponentBits(ParallelKey key, ISimulation simulation, out BitVector32 componentBits);
    }
}

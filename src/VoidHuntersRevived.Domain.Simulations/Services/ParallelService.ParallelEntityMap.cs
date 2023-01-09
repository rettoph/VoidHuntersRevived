using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class ParallelService
    {
        public class ParallelEntityMap
        {
            public static readonly int EmptyEntityId = int.MinValue;

            public ParallelKey Key;

            private IDictionary<SimulationType, int> _entityIds;

            public int this[SimulationType type]
            {
                get => _entityIds[type];
                set => _entityIds[type] = value;
            }

            public bool Empty
            {
                get
                {
                    foreach(int id in _entityIds.Values)
                    {
                        if (id != EmptyEntityId)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            public ParallelEntityMap(ParallelKey key)
            {
                _entityIds = Enum.GetValues<SimulationType>().ToDictionary(
                    keySelector: x => x, 
                    elementSelector: x => EmptyEntityId);

                this.Key = key;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Mappers
{
    public sealed partial class SimulationEntityMapper
    {
        public unsafe class SimulationEntityMap
        {
            public static readonly int EmptyEntityId = int.MinValue;
            public static readonly SimulationType[] MappableSimulationTypes = Enum.GetValues<SimulationType>().Except(SimulationType.None.Yield()).ToArray();

            public readonly int Id;

            private IDictionary<SimulationType, int> _simulationIds;

            public int this[SimulationType type]
            {
                get => _simulationIds[type];
                set => _simulationIds[type] = value;
            }

            public bool Empty
            {
                get
                {
                    foreach(int id in _simulationIds.Values)
                    {
                        if (id != EmptyEntityId)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            public SimulationEntityMap(int id)
            {
                _simulationIds = MappableSimulationTypes.ToDictionary(x => x, x => EmptyEntityId);
                
                this.Id = id;
            }
        }
    }
}

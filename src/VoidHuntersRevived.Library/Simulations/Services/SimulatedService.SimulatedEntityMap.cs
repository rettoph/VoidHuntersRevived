using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Simulations.Services
{
    internal sealed partial class SimulatedService
    {
        public class SimulatedEntityMap
        {
            public static readonly int EmptyEntityId = int.MinValue;
            public static readonly SimulationType[] MappableSimulationTypes = Enum.GetValues<SimulationType>().ToArray();

            public readonly SimulatedId Id;

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

            public SimulatedEntityMap(SimulatedId id)
            {
                _entityIds = MappableSimulationTypes.ToDictionary(x => x, x => EmptyEntityId);
                
                this.Id = id;
            }
        }
    }
}

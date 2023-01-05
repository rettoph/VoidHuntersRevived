using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Simulations.Services
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
                _entityIds = SimulationType.Instances.ToDictionary(
                    keySelector: x => x, 
                    elementSelector: x => EmptyEntityId);

                this.Key = key;
            }
        }
    }
}

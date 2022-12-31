using Guppy.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Maps
{
    public sealed class UserIdSimulatedIdMap
    {
        private Map<int, SimulatedId> _map;

        public UserIdSimulatedIdMap()
        {
            _map = new Map<int, SimulatedId>();
        }

        public void Add(int userId, SimulatedId simulatedId)
        {
            _map.Add(userId, simulatedId);
        }

        public void Remove(int userId)
        {
            _map.Remove(userId);
        }

        public void Remove(SimulatedId simulatedId)
        {
            _map.Remove(simulatedId);
        }

        public bool TryGet(int userId, out SimulatedId simulatedId)
        {
            return _map.TryGet(userId, out simulatedId);
        }

        public bool TryGet(SimulatedId simulatedId, out int userId)
        {
            return _map.TryGet(simulatedId, out userId);
        }

        public SimulatedId Get(int userId)
        {
            if (_map.TryGet(userId, out var simulatedId))
            {
                return simulatedId;
            }

            return default;
        }

        public int Get(SimulatedId simulatedId)
        {
            if (_map.TryGet(simulatedId, out var userId))
            {
                return userId;
            }

            return default;
        }
    }
}

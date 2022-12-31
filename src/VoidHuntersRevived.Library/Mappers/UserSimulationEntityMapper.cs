using Guppy.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Mappers
{
    public sealed class UserSimulationEntityMapper
    {
        private Map<int, int> _map;

        public UserSimulationEntityMapper()
        {
            _map = new Map<int, int>();
        }

        public void Add(int userId, int id)
        {
            _map.Add(userId, id);
        }

        public void RemoveByUserId(int userId)
        {
            _map.Remove(t1: userId);
        }

        public void RemoveById(int id)
        {
            _map.Remove(t2: id);
        }

        public bool TryGetId(int userId, out int id)
        {
            return _map.TryGet(key1: userId, value2: out id);
        }

        public bool TryGetUserId(int id, out int userId)
        {
            return _map.TryGet(key2: id, value1: out userId);
        }

        public int GetId(int userId)
        {
            if(_map.TryGet(key1: userId, value2: out var id))
            {
                return id;
            }

            return default;
        }

        public int GetUserId(int id)
        {
            if (_map.TryGet(key2: id, value1: out var userId))
            {
                return userId;
            }

            return default;
        }
    }
}

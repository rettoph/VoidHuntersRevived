﻿using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Services
{
    public interface IParallelService
    {
        bool TryGetEntityIdFromKey(ParallelKey key, SimulationType type, out int id);

        bool TryGetEntityId(int fromId, SimulationType toType, out int toId);

        int GetIdFromKey(ParallelKey key, SimulationType type);

        int GetId(int fromId, SimulationType toType);

        ParallelKey GetKey(int id);

        void Set(ParallelKey key, SimulationType type, int id);

        void Remove(ParallelKey key, SimulationType type);

        void Remove(SimulationType type, int id);
    }
}

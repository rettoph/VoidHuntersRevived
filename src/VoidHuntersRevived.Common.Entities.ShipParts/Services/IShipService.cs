﻿using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface IShipService
    {
        void MakeShip(Entity entity, Entity bridge, ISimulation simulation);
        Entity CreateShip(ParallelKey key, string bridgeResource, ISimulation simulation);
    }
}

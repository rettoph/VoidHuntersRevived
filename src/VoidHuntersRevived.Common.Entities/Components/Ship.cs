using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public class Ship
    {
        public readonly int EntityId;
        public readonly Entity Bridge;

        public Ship(int entityId, Entity bridge)
        {
            this.EntityId = entityId;
            this.Bridge = bridge;
        }
    }
}

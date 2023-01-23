﻿using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public sealed partial class Pilotable
    {
        public readonly int EntityId;

        public Direction Direction = Direction.None;
        public readonly Aimer Aim = new Aimer();

        public Pilotable(int entityId)
        {
            this.EntityId = entityId;
        }
    }
}

﻿using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public sealed class Tractoring
    {
        public int EntityId;
        public int TargetTreeId;

        public Tractoring(int entityId, int targetId)
        {
            this.EntityId = entityId;
            this.TargetTreeId = targetId;
        }
    }
}

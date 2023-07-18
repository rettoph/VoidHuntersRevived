﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Joint
    {
        public readonly VhId NodeVhId;
        public readonly FixLocation Location;

        public Joint(FixLocation location)
        {
            this.Location = location;
        }
    }
}

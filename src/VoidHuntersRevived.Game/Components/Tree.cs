﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Components
{
    public struct Tree : IEntityComponent
    {
        public static readonly FilterContextID FilterContextID = new FilterContextID();

        public VhId HeadId;
    }
}

﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public delegate void EntityInitializerDelegate(IWorld world, ref EntityInitializer initializer);
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;

namespace VoidHuntersRevived.Common.Physics.Factories
{
    public interface IBodyFactory
    {
        IBody Create(EntityId id);
    }
}

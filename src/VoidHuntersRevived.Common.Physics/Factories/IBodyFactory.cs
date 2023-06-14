﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics.Factories
{
    public interface IBodyFactory
    {
        IBody Create(VhId id);
    }
}

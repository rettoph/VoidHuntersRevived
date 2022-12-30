﻿using Guppy.Attributes;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Simulations.Systems
{
    [GuppyFilter<GameGuppy>()]
    public interface ILockstepSimulationSystem : ISystem
    {
    }
}

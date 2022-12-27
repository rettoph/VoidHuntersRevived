using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Systems
{
    [ConfigurationFilter(SimulationType.Lockstep)]
    [GuppyFilter<GameGuppy>()]
    public interface ILockstepSimulationSystem
    {
    }
}

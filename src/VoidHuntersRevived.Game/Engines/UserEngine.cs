using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : BasicEngine,
        IEventEngine<UserJoined>
    {
        public void Process(VhId id, UserJoined data)
        {
            this.Simulation.World.Entities.Create(EntityNames.UserShip, id.Create(1));
        }
    }
}

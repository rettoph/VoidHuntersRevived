using Guppy.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Pieces;
using VoidHuntersRevived.Game.Pieces.Properties;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class UserEngine : BasicEngine,
        IEventEngine<UserJoined>
    {
        public string name { get; } = nameof(UserEngine);

        public void Process(VhId id, UserJoined data)
        {
            this.Simulation.World.Entities.Create(PieceNames.HullSquare, id.Create(1));
            this.Simulation.World.Entities.Create(PieceNames.HullTriangle, id.Create(1));
        }
    }
}

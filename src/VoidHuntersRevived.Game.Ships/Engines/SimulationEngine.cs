using Guppy.Attributes;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Pieces.Factories;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Pieces;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class SimulationEngine : BasicEngine, IEventEngine<Simulation_Begin>
    {
        private readonly ITreeFactory _treeFactory;

        public SimulationEngine(ITreeFactory treeFactory)
        {
            _treeFactory = treeFactory;
        }

        public void Process(VhId eventId, Simulation_Begin data)
        {
            _treeFactory.Create(eventId.Create(1), EntityTypes.Chain, PieceTypes.HullTriangle);
            _treeFactory.Create(eventId.Create(2), EntityTypes.UserShip, PieceTypes.HullSquare);
        }
    }
}

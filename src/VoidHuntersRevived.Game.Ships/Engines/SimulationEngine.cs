using Guppy.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class SimulationEngine : BasicEngine, IEventEngine<Simulation_Begin>
    {
        private readonly ITreeService _trees;

        public SimulationEngine(ITreeService treeFactory)
        {
            _trees = treeFactory;
        }

        public void Process(VhId eventId, Simulation_Begin data)
        {
            for(int i=0; i<1000; i++)
            {
                _trees.Spawn(eventId.Create(i), TeamId.Default, EntityTypes.Chain, EntityTypes.Pieces.HullTriangle);
            }

            //_trees.Spawn(eventId.Create(2), EntityTypes.UserShip, EntityTypes.Pieces.HullSquare);
        }
    }
}

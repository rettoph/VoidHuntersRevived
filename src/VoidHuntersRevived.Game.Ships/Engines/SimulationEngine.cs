﻿using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
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
        private readonly IPieceService _pieces;

        public SimulationEngine(ITreeService treeFactory, IPieceService pieces)
        {
            _trees = treeFactory;
            _pieces = pieces;
        }

        public void Process(VhId eventId, Simulation_Begin data)
        {
            // for(int j=0; j<1; j++)
            // {
            //     _trees.Spawn(eventId.Create(int.MaxValue), TeamId.TeamZero, EntityTypes.Chain, EntityTypes.Pieces.HullTriangle);
            // }

            int i = 0;
            int radius = 3;
            int step = 2;
            for (int x = -radius; x < radius; x += step)
            {
                for (int y = -radius; y < radius; y += step)
                {
                    _trees.Spawn(eventId.Create(i++), TeamId.TeamZero, EntityTypes.Chain, _pieces.All()[i % _pieces.All().Length].EntityType, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        initializer.Init(new Location()
                        {
                            Position = new FixVector2((Fix64)x, (Fix64)y)
                        });
                    });
                }
            
            }

            //_trees.Spawn(eventId.Create(2), EntityTypes.UserShip, EntityTypes.Pieces.HullSquare);
        }
    }
}

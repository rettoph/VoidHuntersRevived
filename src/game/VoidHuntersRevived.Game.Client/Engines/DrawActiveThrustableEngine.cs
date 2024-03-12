﻿using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal sealed class DrawActiveThrustableEngine : BasicEngine, IStepEngine<GameTimeTeam>
    {
        private readonly short[] _indexBuffer;
        private readonly IEntityService _entities;
        private readonly IPieceTypeService _pieceTypes;
        private readonly ILogger _logger;
        private readonly IResourceProvider _resources;
        private readonly Camera2D _camera;

        public string name { get; } = nameof(DrawVisibleEngine);

        public DrawActiveThrustableEngine(
            ILogger logger,
            IEntityService entities,
            IPieceTypeService pieceTypes,
            IResourceProvider resources,
            Camera2D camera)
        {
            _resources = resources;
            _entities = entities;
            _pieceTypes = pieceTypes;
            _indexBuffer = new short[3];
            _logger = logger;
            _camera = camera;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);
        }

        public void Step(in GameTimeTeam _param)
        {
            // Color activeThrustableHighlight = _resources.Get(Resources.Colors.ActiveThrustableHighlight);
            // _visibleRenderingService.Begin(activeThrustableHighlight, Color.Transparent);
            // foreach (var ((ids, helms, count), groupId) in _entities.QueryEntities<EntityId, Helm>())
            // {
            //     for (int i = 0; i < count; i++)
            //     {
            //         EntityId helmId = ids[i];
            //         Helm helm = helms[i];
            // 
            //         ref var helmThrustables = ref _entities.GetFilter<Thrustable>(helmId, Helm.ThrustableFilterContextId);
            // 
            //         this.TryDrawThrustableImpulse(_param, helm.Direction, ref helmThrustables);
            //     }
            // }
            // 
            // _visibleRenderingService.End();
        }

        private void TryDrawThrustableImpulse(GameTimeTeam param, Direction direction, ref EntityFilterCollection helmThrustables)
        {
            foreach (var (indices, group) in helmThrustables)
            {
                var (pieceTypes, thrustables, nodes, _) = _entities.QueryEntities<Id<PieceType>, Thrustable, Node>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    ref Thrustable thrustable = ref thrustables[index];

                    if ((thrustable.Direction & direction) == 0)
                    {
                        continue;
                    }

                    ref Node node = ref nodes[index];
                    Matrix transformation = node.XnaTransformation;

                    // _visibleRenderingService.Draw(in visibles[index], ref transformation);
                }
            }
        }
    }
}

﻿using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Client.Services;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Components.Static;
using VoidHuntersRevived.Common.Pieces.Enums;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal sealed class DrawActiveThrustableEngine : BasicEngine, IStepEngine<GameTimeTeam>
    {
        private readonly short[] _indexBuffer;
        private readonly IVisibleRenderingService _visibleRenderingService;
        private readonly IEntityService _entities;
        private readonly IPieceTypeService _pieceTypes;
        private readonly ILogger _logger;
        private readonly Dictionary<Id<ITeam>, ITeamDescriptorGroup[]> _teamDescriptorGroups;
        private readonly IResourceProvider _resources;
        private readonly Camera2D _camera;

        public string name { get; } = nameof(DrawVisibleEngine);

        public DrawActiveThrustableEngine(
            ILogger logger,
            IVisibleRenderingService visibleRenderingService,
            IEntityService entities,
            IPieceTypeService pieceTypes,
            ITeamDescriptorGroupService teamDescriptorGroups,
            IResourceProvider resources,
            Camera2D camera)
        {
            _visibleRenderingService = visibleRenderingService;
            _resources = resources;
            _entities = entities;
            _pieceTypes = pieceTypes;
            _indexBuffer = new short[3];
            _logger = logger;
            _camera = camera;

            _teamDescriptorGroups = teamDescriptorGroups.GetAllWithComponentsByTeams(typeof(Visible), typeof(Node));
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);
        }

        public void Step(in GameTimeTeam _param)
        {
            Color activeThrustableHighlight = _resources.Get(Resources.Colors.ActiveThrustableHighlight);
            _visibleRenderingService.Begin(activeThrustableHighlight, Color.Transparent);
            foreach (var ((ids, helms, count), groupId) in _entities.QueryEntities<EntityId, Helm>())
            {
                for (int i = 0; i < count; i++)
                {
                    EntityId helmId = ids[i];
                    Helm helm = helms[i];

                    ref var helmThrustables = ref _entities.GetFilter<Thrustable>(helmId, Helm.ThrustableFilterContextId);

                    this.TryDrawThrustableImpulse(_param, helm.Direction, ref helmThrustables);
                }
            }

            _visibleRenderingService.End();
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

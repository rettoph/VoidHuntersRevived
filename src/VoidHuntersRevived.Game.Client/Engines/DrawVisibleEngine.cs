using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using System.Drawing;
using VoidHuntersRevived.Common.Client.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Physics.Extensions.FixedPoint;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using Guppy.Game.Common.Enums;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [GuppyFilter<LocalGameGuppy>]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal sealed class DrawVisibleEngine : BasicEngine, IStepEngine<GameTimeTeam>
    {
        private readonly short[] _indexBuffer;
        private readonly IVisibleRenderingService _visibleRenderingService;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;
        private readonly Dictionary<Id<ITeam>, ITeamDescriptorGroup[]> _teamDescriptorGroups;
        private readonly Camera2D _camera;

        public string name { get; } = nameof(DrawVisibleEngine);

        public DrawVisibleEngine(
            ILogger logger, 
            IVisibleRenderingService visibleRenderingService, 
            IEntityService entities, 
            ITeamDescriptorGroupService teamDescriptorGroups,
            Camera2D camera)
        {
            _visibleRenderingService = visibleRenderingService;
            _entities = entities;
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
            var bounds = _camera.Frustum.ToBounds2D();

            foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
            {
                var (statuses, visibles, nodes, count) = _entities.QueryEntities<EntityStatus, Visible, Node>(teamDescriptorGroup.GroupId);

                _visibleRenderingService.Begin(teamDescriptorGroup.PrimaryColor, teamDescriptorGroup.SecondaryColor);

                for (int index = 0; index < count; index++)
                {
                    try
                    {
                        if (statuses[index].IsSpawned)
                        {
                            ref Node node = ref nodes[index];
                            Matrix transformation = node.XnaTransformation;
                            
                            if(bounds.Contains(transformation))
                            {
                                _visibleRenderingService.Draw(in visibles[index], ref transformation);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        var (ids, _) = _entities.QueryEntities<EntityId>(teamDescriptorGroup.GroupId);
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(DrawVisibleEngine), nameof(Step), ids[index].VhId.Value);
                    }
                }
                _visibleRenderingService.End();
            }
        }
    }
}

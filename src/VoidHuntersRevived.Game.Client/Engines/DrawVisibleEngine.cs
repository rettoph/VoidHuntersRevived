using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Client.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationTypeFilter(SimulationType.Predictive)]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.Draw)]
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
            if (_camera.Zoom > 20)
            {
                this.DrawHighResolution(_param.Team.Id);
            }
            else
            {
                this.DrawLowResolution(_param.Team.Id);
            }
        }

        private void DrawHighResolution(Id<ITeam> teamId)
        {
            foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[teamId])
            {
                var (statuses, visibles, nodes, count) = _entities.QueryEntities<EntityStatus, Visible, Node>(teamDescriptorGroup.GroupId);


                for (int index = 0; index < count; index++)
                {
                    try
                    {
                        if (statuses[index].IsSpawned)
                        {
                            Matrix transformation = nodes[index].Transformation.ToTransformationXnaMatrix();

                            if (!_camera.Contains(transformation))
                            {
                                continue;
                            }

                            _visibleRenderingService.BeginFill();
                            _visibleRenderingService.Fill(in visibles[index], ref transformation, teamDescriptorGroup.PrimaryColor);
                            _visibleRenderingService.End();

                            _visibleRenderingService.BeginTrace();
                            _visibleRenderingService.Trace(in visibles[index], ref transformation, teamDescriptorGroup.SecondaryColor);
                            _visibleRenderingService.End();
                        }
                    }
                    catch (Exception e)
                    {
                        var (ids, _) = _entities.QueryEntities<EntityId>(teamDescriptorGroup.GroupId);
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(DrawVisibleEngine), nameof(Step), ids[index].VhId.Value);
                    }
                }
            }
        }

        private void DrawLowResolution(Id<ITeam> teamId)
        {
            foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[teamId])
            {
                var (statuses, visibles, nodes, count) = _entities.QueryEntities<EntityStatus, Visible, Node>(teamDescriptorGroup.GroupId);

                _visibleRenderingService.BeginFill();
                for (int index = 0; index < count; index++)
                {
                    try
                    {
                        if (statuses[index].IsSpawned)
                        {
                            Matrix transformation = nodes[index].Transformation.ToTransformationXnaMatrix();

                            if (_camera.Frustum.Contains(transformation.GetBoudingSphere(5f)) == ContainmentType.Disjoint)
                            {
                                continue;
                            }


                            _visibleRenderingService.Fill(in visibles[index], ref transformation, teamDescriptorGroup.PrimaryColor);
                        }
                    }
                    catch (Exception e)
                    {
                        var (ids, _) = _entities.QueryEntities<EntityId>(teamDescriptorGroup.GroupId);
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(DrawVisibleEngine), nameof(Step), ids[index].VhId.Value);
                    }
                }
                _visibleRenderingService.End();

                _visibleRenderingService.BeginTrace();
                for (int index = 0; index < count; index++)
                {
                    try
                    {
                        if (statuses[index].IsSpawned)
                        {
                            Matrix transformation = nodes[index].Transformation.ToTransformationXnaMatrix();

                            if (_camera.Frustum.Contains(transformation.GetBoudingSphere(5f)) == ContainmentType.Disjoint)
                            {
                                continue;
                            }

                            _visibleRenderingService.Trace(in visibles[index], ref transformation, teamDescriptorGroup.SecondaryColor);
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

using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Client.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Components.Static;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [GuppyFilter<LocalGameGuppy>]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal sealed class DrawVisibleEngine : BasicEngine, IStepEngine<GameTimeTeam>, IStepEngine<GameTime>
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

            foreach (var ((visibles, zIndices, count), _) in _entities.QueryEntities<Visible, zIndex>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref Visible visible = ref visibles[i];
                }
            }
        }

        public void Step(in GameTimeTeam _param)
        {
            // var bounds = _camera.Frustum.ToBounds2D();
            // 
            // foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
            // {
            //     var (statuses, visibles, nodes, count) = _entities.QueryEntities<EntityStatus, Visible, Node>(teamDescriptorGroup.GroupId);
            // 
            //     _visibleRenderingService.Begin(teamDescriptorGroup.PrimaryColor, teamDescriptorGroup.SecondaryColor);
            // 
            //     for (int index = 0; index < count; index++)
            //     {
            //         try
            //         {
            //             if (statuses[index].IsSpawned)
            //             {
            //                 ref Node node = ref nodes[index];
            //                 Matrix transformation = node.XnaTransformation;
            // 
            //                 if (bounds.Contains(transformation))
            //                 {
            //                     _visibleRenderingService.Draw(in visibles[index], ref transformation);
            //                 }
            //                 else
            //                 {
            // 
            //                 }
            //             }
            //         }
            //         catch (Exception e)
            //         {
            //             var (ids, _) = _entities.QueryEntities<EntityId>(teamDescriptorGroup.GroupId);
            //             _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(DrawVisibleEngine), nameof(Step), ids[index].VhId.Value);
            //         }
            //     }
            //     _visibleRenderingService.End();
            // }
        }

        public void Step(in GameTime param)
        {
            return;
            foreach (var ((statics, static_visibles, static_count), static_group) in _entities.QueryEntities<StaticEntity, Visible>())
            {
                for (int i_static = 0; i_static < static_count; i_static++)
                {
                    Visible static_visible = static_visibles[i_static];
                    ref var instance_filter = ref _entities.GetFilter<EntityId>(statics[i_static].InstanceEntitiesFilterId);

                    foreach (var (instances_indices, instances_group) in instance_filter)
                    {
                        var (locations, _) = _entities.QueryEntities<Location>(instances_group);

                        for (int instance_i = 0; instance_i < instances_indices.count; instance_i++)
                        {
                            uint index = instances_indices[instance_i];
                            Location location = locations[index];


                        }
                    }
                }
            }
        }
    }
}

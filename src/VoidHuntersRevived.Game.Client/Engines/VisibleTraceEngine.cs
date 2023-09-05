using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.GUI;
using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces;
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
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PostDraw)]
    internal sealed class VisibleTraceEngine : BasicEngine, IStepEngine<GameTimeTeam>
    {
        private readonly short[] _indexBuffer;
        private readonly IVisibleRenderingService _visibleRenderingService;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;
        private readonly Dictionary<TeamId, ITeamDescriptorGroup[]> _teamDescriptorGroups;

        public string name { get; } = nameof(VisibleFillEngine);

        public VisibleTraceEngine(
            ILogger logger, 
            IVisibleRenderingService visibleRenderingService, 
            IEntityService entities, 
            ITeamDescriptorGroupService teamDescriptorGroups)
        {
            _visibleRenderingService = visibleRenderingService;
            _entities = entities;
            _indexBuffer = new short[3];
            _logger = logger;

            _teamDescriptorGroups = teamDescriptorGroups.GetAllWithComponentsByTeams(typeof(Visible), typeof(Node));
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);
        }

        public void Step(in GameTimeTeam _param)
        {
            _visibleRenderingService.BeginTrace();
            foreach (ITeamDescriptorGroup teamDescriptorGroup in _teamDescriptorGroups[_param.Team.Id])
            {
                var (ids, statuses, visibles, nodes, count) = _entities.QueryEntities<EntityId, EntityStatus, Visible, Node>(teamDescriptorGroup.GroupId);
                for (int index = 0; index < count; index++)
                {
                    try
                    {
                        if (statuses[index].IsSpawned)
                        {
                            Matrix transformation = nodes[index].Transformation.XnaMatrix;
                            _visibleRenderingService.Trace(in visibles[index], ref transformation, teamDescriptorGroup.Color);
                            //_visibleRenderingService.Fill(in visibles[i], ref transformation, this.Simulation.Type == SimulationType.Predictive ? Color.Green : Color.Red);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(VisibleFillEngine), nameof(Step), ids[index].VhId.Value);
                    }
                }
            }
            _visibleRenderingService.End();
        }
    }
}

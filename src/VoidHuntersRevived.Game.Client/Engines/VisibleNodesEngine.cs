using Guppy.Attributes;
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
using System.Text;
using System.Threading.Tasks;
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

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class VisibleNodesEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly short[] _indexBuffer;
        private readonly IVisibleRenderingService _visibleRenderingService;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public string name { get; } = nameof(VisibleNodesEngine);

        public VisibleNodesEngine(ILogger logger, IVisibleRenderingService visibleRenderingService, IEntityService entities)
        {
            _visibleRenderingService = visibleRenderingService;
            _entities = entities;
            _indexBuffer = new short[3];
            _logger = logger;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);
        }

        public void Step(in GameTime _param)
        {
            _visibleRenderingService.BeginFill();
            foreach (var ((ids, statuses, visibles, nodes, count), _) in _entities.QueryEntities<EntityId, EntityStatus, Visible, Node>())
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        if (statuses[i].IsSpawned)
                        {
                            Matrix transformation = nodes[i].Transformation.XnaMatrix;
                            _visibleRenderingService.Fill(in visibles[i], ref transformation);
                        }
                    }
                    catch(Exception e)
                    {
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to fill shapes for visible {VisibleVhId}", nameof(VisibleNodesEngine), nameof(Step), ids[i].VhId.Value);
                    }
                }
            }
            _visibleRenderingService.End();

            _visibleRenderingService.BeginTrace();
            foreach (var ((ids, statuses, visibles, nodes, count), _) in _entities.QueryEntities<EntityId, EntityStatus, Visible, Node>())
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        if (statuses[i].IsSpawned)
                        {
                            Matrix transformation = nodes[i].Transformation.XnaMatrix;
                            _visibleRenderingService.Trace(in visibles[i], ref transformation);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "{ClassName}::{MethodName} - Exception attempting to trace paths for visible {VisibleVhId}", nameof(VisibleNodesEngine), nameof(Step), ids[i].VhId.Value);
                    }
                }
            }
            _visibleRenderingService.End();
        }
    }
}

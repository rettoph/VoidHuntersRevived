using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Client.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [GuppyFilter<LocalGameGuppy>]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal sealed class DrawVisibleEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly short[] _indexBuffer;
        private readonly IVisibleInstanceRenderingService _visibleInstanceRenderingService;
        private readonly IEntityService _entities;
        private readonly IEntityTypeService _types;
        private readonly ILogger _logger;
        private readonly Camera2D _camera;

        public string name { get; } = nameof(DrawVisibleEngine);

        public DrawVisibleEngine(
            ILogger logger,
            IVisibleInstanceRenderingService visibleInstanceRenderingService,
            IEntityService entities,
            IEntityTypeService types,
            Camera2D camera)
        {
            _visibleInstanceRenderingService = visibleInstanceRenderingService;
            _entities = entities;
            _types = types;
            _indexBuffer = new short[3];
            _logger = logger;
            _camera = camera;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);
        }

        public void Step(in GameTime param)
        {
            // _visibleInstanceRenderingService.DrawAll();
            // return;

            _visibleInstanceRenderingService.Begin();

            foreach (var ((statics, entityTypes, _, _, count), _) in _entities.QueryEntities<StaticEntity, Id<IEntityType>, Visible, zIndex>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref StaticEntity @static = ref statics[i];
                    ref Id<IEntityType> entityType = ref entityTypes[i];

                    ref var instanceFilter = ref _entities.GetFilter<EntityId>(@static.InstanceEntitiesFilterId);

                    _visibleInstanceRenderingService.Draw(entityType, @static.InstanceEntitiesCount, ref instanceFilter);
                }
            }

            _visibleInstanceRenderingService.End();
        }
    }
}

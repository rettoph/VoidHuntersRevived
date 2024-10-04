using Guppy.Core.Common.Attributes;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    public class DrawVertexVisibleEngine(
        IEntityQueryService entityQueryService,
        Camera2D camera,
        GraphicsDevice graphics) : StrategyEngine, IOnStepEngine
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly Camera2D _camera = camera;
        private readonly GraphicsDevice _graphics = graphics;

        /// <summary>
        /// Copy Svelto entity data to vertex
        /// </summary>
        /// <param name="param"></param>
        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            foreach (var ((vertices, colorSchemes, nodes, _, count), _) in _entityQueryService.QueryEntities<VertexVisible, ColorScheme, Node>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref VertexVisible vertex = ref vertices[i];
                    ref ColorScheme colorScheme = ref colorSchemes[i];
                    ref Node node = ref nodes[i];

                    vertex.LocalTransformation = node.XnaTransformation;
                    vertex.PrimaryColor = colorScheme.Primary.Value.PackedValue;
                    vertex.SecondaryColor = colorScheme.Secondary.Value.PackedValue;
                }
            }
        }
    }
}

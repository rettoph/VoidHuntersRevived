using Guppy.Core.Common.Attributes;
using Guppy.Game.Graphics.Common;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Game.Client.Engines
{
    public class DrawVertexVisibleEngine(
        IEntityQueryService entityQueryService,
        ICamera2D camera,
        GraphicsDevice graphics) : StrategyEngine,
            IGraphicsEngine,
            IOnStepEngine,
            IOnSpawnEngine<VertexVisible>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ICamera2D _camera = camera;
        private readonly GraphicsDevice _graphics = graphics;

        /// <summary>
        /// Copy Initial Svelto data to vertex on spawn
        /// </summary>
        /// <param name="sourceEventId"></param>
        /// <param name="entityTemplate"></param>
        /// <param name="id"></param>
        /// <param name="component"></param>
        /// <param name="groupIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group05)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<VertexVisible> vertexVisible)
        {
            var (vertices, colorSchemes, nodes, _, _) = _entityQueryService.QueryEntities<VertexVisible, ColorScheme, Node>(vertexVisible.Group);

            ref VertexVisible vertex = ref vertices[vertexVisible.Index];
            ref ColorScheme colorScheme = ref colorSchemes[vertexVisible.Index];
            ref Node node = ref nodes[vertexVisible.Index];

            vertex.LocalTransformation = node.XnaTransformation;
            vertex.PrimaryColor = colorScheme.Primary.Value.PackedValue;
            vertex.SecondaryColor = colorScheme.Secondary.Value.PackedValue;
        }

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

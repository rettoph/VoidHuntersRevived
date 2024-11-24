using Guppy.Core.Common.Attributes;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Graphics.Common.Attributes;
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
    [GraphicsEnabled]
    public class DrawVertexVisibleEngine(
        IEntityQueryService entityQueryService,
        ICamera2D camera,
        GraphicsDevice graphics) : StrategyEngine, IOnStepEngine, IOnSpawnEngine<VertexVisible>
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
        public void OnSpawn(VhId sourceEventId, IEntityTemplate entityTemplate, EntityId id, ref VertexVisible component, in GroupIndex groupIndex)
        {
            var (vertices, colorSchemes, nodes, _, _) = _entityQueryService.QueryEntities<VertexVisible, ColorScheme, Node>(groupIndex.GroupID);

            ref VertexVisible vertex = ref vertices[groupIndex.Index];
            ref ColorScheme colorScheme = ref colorSchemes[groupIndex.Index];
            ref Node node = ref nodes[groupIndex.Index];

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

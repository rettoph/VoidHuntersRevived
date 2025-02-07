using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Game.Client.Systems
{
    public class DrawVertexVisibleSystem(
        IEntityQueryService entityQueryService
    ) : ISceneSystem,
        IStepSystem,
        IOnSpawnSystem<VertexVisible>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

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
            var (vertices, colorSchemes, fixtures, _, _) = this._entityQueryService.QueryEntities<VertexVisible, ColorScheme, Fixture>(vertexVisible.Group);

            ref VertexVisible vertex = ref vertices[vertexVisible.Index];
            ref ColorScheme colorScheme = ref colorSchemes[vertexVisible.Index];
            ref Fixture fixture = ref fixtures[vertexVisible.Index];

            vertex.LocalTransformation = fixture.WorldTransform.ToMatrix();
            vertex.PrimaryColor = colorScheme.Primary.Value.PackedValue;
            vertex.SecondaryColor = colorScheme.Secondary.Value.PackedValue;
        }

        /// <summary>
        /// Copy Svelto entity data to vertex
        /// </summary>
        /// <param name="param"></param>
        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.SyncronizeEntities)]
        public void Step(Step step)
        {
            foreach (var ((vertices, colorSchemes, fixtures, _, count), _) in this._entityQueryService.QueryEntities<VertexVisible, ColorScheme, Fixture>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref VertexVisible vertex = ref vertices[i];
                    ref ColorScheme colorScheme = ref colorSchemes[i];
                    ref Fixture fixture = ref fixtures[i];

                    vertex.LocalTransformation = fixture.WorldTransform.ToMatrix();
                    vertex.PrimaryColor = colorScheme.Primary.Value.PackedValue;
                    vertex.SecondaryColor = colorScheme.Secondary.Value.PackedValue;
                }
            }
        }
    }
}
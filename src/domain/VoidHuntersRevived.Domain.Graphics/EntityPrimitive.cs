using Guppy.Core.Common.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics
{
    internal class EntityPrimitive<TVertexInstance, TVertexStatic, TEffect>(
            int sequence,
            PrimitiveSequenceGroupEnum sequenceGroup,
            VertexBuffer staticVertexBuffer,
            IndexBuffer[] staticIndexBuffers,
            PrimitiveTypeEnum[] bufferTypes,
            TEffect effect,
            GraphicsDevice graphics
        ) : Primitive<TVertexInstance, TVertexStatic, TEffect>(sequence, sequenceGroup, staticVertexBuffer, staticIndexBuffers, bufferTypes, effect, graphics),
            IRuntimeSequence<PrimitiveSequenceGroupEnum>,
            IRuntimeSequenceGroup<PrimitiveSequenceGroupEnum>
        where TVertexInstance : unmanaged, IVertexType, IEntityComponent
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
        public override void Draw(EntitiesDB entitiesDb)
        {
            this.CopyEntityDataToVertexBuffer(entitiesDb);

            base.Draw(entitiesDb);
        }

        private void CopyEntityDataToVertexBuffer(EntitiesDB entitiesDb)
        {
            ref EntityFilterCollection filter = ref this.GetFilter<TVertexInstance>(entitiesDb);

            foreach (var (indices, group) in filter)
            {
                this.EnsureFit(indices.count);

                var (vertices, statuses, _) = entitiesDb.QueryEntities<TVertexInstance, EntityStatus>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    if (statuses[index].IsDespawned)
                    { // Dont render pieces that have been despawned
                        continue;
                    }

                    this.SetNextVertexUnsafe(vertices[index]);
                }
            }
        }
    }
}

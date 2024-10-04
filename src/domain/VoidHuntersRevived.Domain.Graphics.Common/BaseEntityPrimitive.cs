using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Contexts;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public abstract class BaseEntityPrimitive<TVertex>(PrimitiveContext context, PrimitiveSequenceGroupEnum sequenceGroup, GraphicsDevice graphics, BufferContext staticBufferContext, Effect effect) : BasePrimitive<TVertex>(context, sequenceGroup, graphics, staticBufferContext, effect), IPrimitive<TVertex>, IQueryingEntitiesEngine
        where TVertex : unmanaged, IVertexType, IEntityComponent
    {
        public override void Draw(GameTime gameTime)
        {
            this.CopyEntityDataToVertexBuffer();

            base.Draw(gameTime);
        }

        public void CopyEntityDataToVertexBuffer()
        {
            ref EntityFilterCollection filter = ref this.GetFilter<TVertex>();

            foreach (var (indices, group) in filter)
            {
                this.EnsureFit(indices.count);

                var (vertices, statuses, _) = this.entitiesDB.QueryEntities<TVertex, EntityStatus>(group);

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

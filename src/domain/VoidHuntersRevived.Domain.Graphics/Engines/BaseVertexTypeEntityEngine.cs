using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    public abstract class BaseVertexTypeEntityEngine<TVertex> : StrategyEngine,
        IReactOnAddEx<TVertex>, IStepEngine<GameTime>, IQueryingEntitiesEngine
        where TVertex : unmanaged, IVertexType, IEntityComponent
    {
        public string name => nameof(BaseVertexTypeEntityEngine<TVertex>);

        public EntitiesDB entitiesDB { get; set; } = null!;

        protected void CopyEntityVertexData(IVertexBuffer<TVertex> vertexBuffer)
        {
            ref EntityFilterCollection filter = ref vertexBuffer.GetFilter<TVertex>();

            foreach (var (indices, group) in filter)
            {
                vertexBuffer.EnsureFit(indices.count);

                var (vertices, statuses, _) = this.entitiesDB.QueryEntities<TVertex>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    vertexBuffer.SetNextVertexUnsafe(vertices[index]);
                }
            }
        }

        protected void CopySpawnedEntityVertexData(IVertexBuffer<TVertex> vertexBuffer)
        {
            ref EntityFilterCollection filter = ref vertexBuffer.GetFilter<TVertex>();

            foreach (var (indices, group) in filter)
            {
                vertexBuffer.EnsureFit(indices.count);

                var (vertices, statuses, _) = this.entitiesDB.QueryEntities<TVertex, EntityStatus>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    if (statuses[index].IsDespawned)
                    { // Dont render pieces that have been despawned
                        continue;
                    }

                    vertexBuffer.SetNextVertexUnsafe(vertices[index]);
                }
            }
        }

        public abstract void Step(in GameTime param);

        public abstract void Add((uint start, uint end) rangeOfEntities, in EntityCollection<TVertex> entities, ExclusiveGroupStruct groupID);
    }
}

using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    public sealed class PrimitiveEntity_ManyTypesSingleGroup_Engine<TVertex> : BaseVertexTypeEntityEngine<TVertex>
        where TVertex : unmanaged, IVertexType, IEntityComponent
    {
        private readonly IVertexTypeManager<TVertex> _vertexTypeManager;
        private readonly Dictionary<Key<IEntityType>, IVertexBuffer<TVertex>> _vertexBuffersByType;
        private readonly IVertexBuffer<TVertex>[] _vertexBuffers;

        public PrimitiveEntity_ManyTypesSingleGroup_Engine(IVertexTypeManagerProvider<TVertex> vertexTypeManagerProvider)
        {
            _vertexTypeManager = vertexTypeManagerProvider.GetAll().Single();
            _vertexBuffersByType = vertexTypeManagerProvider.Primitives.ToDictionary(x => x.EntityTypeKey ?? throw new NotImplementedException(), x => x.GetAllVertexBuffers().Single().Value);
            _vertexBuffers = _vertexBuffersByType.Values.ToArray();
        }

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.PreDraw)]
        public override void OnDraw(GameTime gameTime)
        {
            foreach (IVertexBuffer<TVertex> vertexBuffer in _vertexBuffers)
            {
                this.CopySpawnedEntityVertexData(vertexBuffer);
            }
        }

        public override void Add((uint start, uint end) rangeOfEntities, in EntityCollection<TVertex> entities, ExclusiveGroupStruct groupID)
        {
            var (_, nativeIds, _) = entities;
            var (instances, _) = this.entitiesDB.QueryEntities<EntityType>(groupID);

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                EntityType instance = instances[i];
                _vertexBuffersByType[instance.Value.Key].GetFilter<TVertex>().Add(nativeIds[i], groupID, i);
            }
        }
    }
}

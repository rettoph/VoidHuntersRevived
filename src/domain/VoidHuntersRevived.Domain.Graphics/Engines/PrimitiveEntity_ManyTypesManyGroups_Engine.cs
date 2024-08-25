using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    public sealed class PrimitiveEntity_ManyTypesManyGroups_Engine<TVertex> : BaseVertexTypeEntityEngine<TVertex>
        where TVertex : unmanaged, IVertexType, IEntityComponent
    {
        private readonly Dictionary<IKey<IEntityType>, IPrimitive<TVertex>> _primitivesByType;
        private readonly IVertexBuffer<TVertex>[] _vertexBuffers;

        public PrimitiveEntity_ManyTypesManyGroups_Engine(IVertexTypeManagerProvider<TVertex> vertexTypeManagerProvider)
        {
            _primitivesByType = vertexTypeManagerProvider.Primitives.ToDictionary(x => x.EntityTypeKey ?? throw new NotImplementedException(), x => x);
            _vertexBuffers = vertexTypeManagerProvider.Primitives.SelectMany(x => x.GetAllVertexBuffers()).Select(x => x.Value).ToArray();
        }

        public override void Step(in GameTime param)
        {
            foreach (IVertexBuffer<TVertex> vertexBuffer in _vertexBuffers)
            {
                this.CopySpawnedEntityVertexData(vertexBuffer);
            }
        }

        public override void Add((uint start, uint end) rangeOfEntities, in EntityCollection<TVertex> entities, ExclusiveGroupStruct groupID)
        {
            var (_, nativeIds, _) = entities;
            var (instances, groups, _) = this.entitiesDB.QueryEntities<Entities.Common.Components.EntityType, PrimitiveGroup>(groupID);

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                Entities.Common.Components.EntityType instance = instances[i];
                PrimitiveGroup group = groups[i];

                _primitivesByType[instance.Value.Key].GetVertexBuffer(group.Value).GetFilter<TVertex>().Add(nativeIds[i], groupID, i);
            }
        }
    }
}

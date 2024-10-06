using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Components
{
    public readonly struct PrimitiveEntity<TVertex>(Key<PrimitiveType> primitiveType, PrimitiveSequenceGroupEnum sequenceGroup) : IEntityComponent
        where TVertex : unmanaged, IVertexType
    {
        public readonly Key<PrimitiveType> Type = primitiveType;
        public readonly PrimitiveSequenceGroupEnum SequenceGroup = sequenceGroup;
    }
}

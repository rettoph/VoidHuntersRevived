using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Components
{
    public readonly struct PrimitiveComponent<TVertex>(Key<IPrimitiveType> primitiveType, PrimitiveSequenceGroupEnum sequenceGroup) : IEntityComponent
        where TVertex : unmanaged, IVertexType
    {
        public readonly Key<IPrimitiveType> Type = primitiveType;
        public readonly PrimitiveSequenceGroupEnum SequenceGroup = sequenceGroup;
    }
}

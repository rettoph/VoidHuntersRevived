using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Components
{
    public readonly struct PrimitiveSequenceGroup<TVertex>(PrimitiveSequenceGroupEnum value) : IEntityComponent
        where TVertex : unmanaged, IVertexType
    {
        public readonly PrimitiveSequenceGroupEnum Value = value;
    }
}

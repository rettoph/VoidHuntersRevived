using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Graphics.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(PrimitiveSequence))]
    public struct PrimitiveSequence : IEntityComponent
    {
        public int Value { get; set; }
    }
}

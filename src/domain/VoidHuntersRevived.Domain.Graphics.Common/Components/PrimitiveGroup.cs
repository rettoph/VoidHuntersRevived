using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(PrimitiveGroup))]
    public struct PrimitiveGroup(PrimitiveGroupEnum group) : IEntityComponent
    {
        public PrimitiveGroupEnum Value { get; set; } = group;

        public PrimitiveGroup() : this(PrimitiveGroupEnum.None)
        {

        }
    }
}

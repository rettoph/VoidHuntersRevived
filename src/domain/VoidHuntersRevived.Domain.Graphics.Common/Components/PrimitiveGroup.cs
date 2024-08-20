using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(PrimitiveGroup))]
    public struct PrimitiveGroup : IEntityComponent
    {
        public PrimitiveGroupEnum Value { get; set; }

        public PrimitiveGroup() : this(PrimitiveGroupEnum.None)
        {

        }
        public PrimitiveGroup(PrimitiveGroupEnum group)
        {
            this.Value = group;
        }
    }
}

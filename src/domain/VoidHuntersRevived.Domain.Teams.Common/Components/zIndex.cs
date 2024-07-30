using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(zIndex))]
    public struct zIndex : IEntityComponent
    {
        public readonly short Value;

        public zIndex(short value)
        {
            this.Value = value;
        }
    }
}

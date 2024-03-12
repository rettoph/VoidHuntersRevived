using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Options
{
    public unsafe struct DeserializationOptions
    {
        public VhId Seed { get; init; }
        public VhId Owner { get; init; }
    }
}

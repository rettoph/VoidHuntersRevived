using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Options
{
    public readonly struct DeserializationOptions
    {
        public required VhId Seed { get; init; }
        public required VhId Owner { get; init; }
    }
}

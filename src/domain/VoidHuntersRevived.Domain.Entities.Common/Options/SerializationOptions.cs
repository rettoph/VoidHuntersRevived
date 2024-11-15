using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Options
{
    public readonly struct SerializationOptions
    {
        public static SerializationOptions Default { get; } = new SerializationOptions()
        {
            Recursion = Recursion.All
        };

        public Recursion Recursion { get; init; }
    }
}

using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Options
{
    public struct SerializationOptions
    {
        public static SerializationOptions Default = new SerializationOptions()
        {
            Recursion = Recursion.All
        };

        public Recursion Recursion { get; init; }
    }
}

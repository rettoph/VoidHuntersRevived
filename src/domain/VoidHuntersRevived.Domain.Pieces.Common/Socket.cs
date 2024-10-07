using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public struct Socket(Location location)
    {
        public Location Location { get; set; } = location;
    }
}

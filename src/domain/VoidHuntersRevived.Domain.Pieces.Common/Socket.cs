using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public struct Socket
    {
        public Location Location { get; set; }

        public Socket(Location location)
        {
            this.Location = location;
        }
    }
}

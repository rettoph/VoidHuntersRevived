using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Entities.Enums;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public sealed partial class Pilotable
    {
        public Direction Direction = Direction.None;
        public readonly Aimer Aim = new Aimer();
    }
}

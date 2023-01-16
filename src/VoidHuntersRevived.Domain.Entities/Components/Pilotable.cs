using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Enums;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public sealed class Pilotable
    {
        public Direction Direction = Direction.None;
        public Vector2 Target = Vector2.Zero;
    }
}

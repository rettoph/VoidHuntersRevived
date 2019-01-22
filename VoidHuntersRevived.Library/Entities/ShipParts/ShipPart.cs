using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract class ShipPart : FarseerEntity
    {
        public ShipPart(EntityInfo info, IGame game) : base(info, game)
        {
        }
    }
}

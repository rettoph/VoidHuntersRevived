using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public class Drawn : IShipPartComponent
    {
        public string Color { get; set; } = ShipPartColors.Default;

        public Vector2[][] Shapes { get; set; } = Array.Empty<Vector2[]>();
        public Vector2[][] Paths { get; set; } = Array.Empty<Vector2[]>();
    }
}

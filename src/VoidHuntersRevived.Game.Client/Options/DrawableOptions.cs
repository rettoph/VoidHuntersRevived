using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Game.Client.Options
{
    public class DrawableOptions<TComponent>
    {
        public bool Visible { get; set; }
        public Color? Tint { get; set; }
    }
}

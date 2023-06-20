using Guppy.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public static class Resources
    {
        public static class Fonts
        {
            public static readonly Resource<SpriteFont> Default = new Resource<SpriteFont>($"{nameof(Fonts)}.{nameof(Default)}");
        }

        public static class Colors
        {
            public static readonly Resource<Color> Orange = new Resource<Color>($"{nameof(Colors)}.{nameof(Orange)}");
        }
    }
}

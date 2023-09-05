using Guppy.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Common
{
    public static class Resources
    {
        public static class Strings
        {
            public static readonly Resource<string> DefaultTeam = new Resource<string>($"{nameof(Strings)}.{nameof(DefaultTeam)}");
            public static readonly Resource<string> BlueTeam = new Resource<string>($"{nameof(Strings)}.{nameof(BlueTeam)}");
        }

        public static class Fonts
        {
            public static readonly Resource<SpriteFont> Default = new Resource<SpriteFont>($"{nameof(Fonts)}.{nameof(Default)}");
        }

        public static class Colors
        {
            public static readonly Resource<Color> None = new Resource<Color>($"{nameof(Colors)}.{nameof(None)}");

            public static readonly Resource<Color> Orange = new Resource<Color>($"{nameof(Colors)}.{nameof(Orange)}");
            public static readonly Resource<Color> Blue = new Resource<Color>($"{nameof(Colors)}.{nameof(Blue)}");

            public static readonly Resource<Color> TractorBeamHighlight = new Resource<Color>($"{nameof(Colors)}.{nameof(TractorBeamHighlight)}");
        }
    }
}

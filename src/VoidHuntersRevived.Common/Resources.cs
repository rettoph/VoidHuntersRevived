using Guppy.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Common
{
    public static class Resources
    {
        public static class Strings
        {
            public static readonly Resource<string> TeamZeroName = new Resource<string>($"{nameof(Strings)}.{nameof(TeamZeroName)}");
            public static readonly Resource<string> TeamOneName = new Resource<string>($"{nameof(Strings)}.{nameof(TeamOneName)}");
        }

        public static class Fonts
        {
            public static readonly Resource<SpriteFont> Default = new Resource<SpriteFont>($"{nameof(Fonts)}.{nameof(Default)}");
        }

        public static class Colors
        {
            public static readonly Resource<Color> None = new Resource<Color>($"{nameof(Colors)}.{nameof(None)}");

            public static readonly Resource<Color> HullPrimaryColor = new Resource<Color>($"{nameof(Colors)}.{nameof(HullPrimaryColor)}");
            public static readonly Resource<Color> HullSecondaryColor = new Resource<Color>($"{nameof(Colors)}.{nameof(HullSecondaryColor)}");

            public static readonly Resource<Color> ThrusterPrimaryColor = new Resource<Color>($"{nameof(Colors)}.{nameof(ThrusterPrimaryColor)}");
            public static readonly Resource<Color> ThrusterSecondaryColor = new Resource<Color>($"{nameof(Colors)}.{nameof(ThrusterSecondaryColor)}");

            public static readonly Resource<Color> TeamOnePrimaryColor = new Resource<Color>($"{nameof(Colors)}.{nameof(TeamOnePrimaryColor)}");
            public static readonly Resource<Color> TeamOneSecondaryColor = new Resource<Color>($"{nameof(Colors)}.{nameof(TeamOneSecondaryColor)}");

            public static readonly Resource<Color> TractorBeamHighlight = new Resource<Color>($"{nameof(Colors)}.{nameof(TractorBeamHighlight)}");
        }
    }
}

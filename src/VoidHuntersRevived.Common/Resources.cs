using Guppy.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Common
{
    public static class Resources
    {
        public static class Strings
        {
            public static readonly Resource<string> TeamZeroName = Resource.Get<string>($"{nameof(String)}.{nameof(TeamZeroName)}");
            public static readonly Resource<string> TeamOneName = Resource.Get<string>($"{nameof(String)}.{nameof(TeamOneName)}");
        }

        public static class SpriteFonts
        {
            public static readonly Resource<SpriteFont> Default = Resource.Get<SpriteFont>($"{nameof(SpriteFont)}.{nameof(Default)}");
        }

        public static class Colors
        {
            public static readonly Resource<Color> None = Resource.Get<Color>($"{nameof(Color)}.{nameof(None)}");

            public static readonly Resource<Color> HullPrimaryColor = Resource.Get<Color>($"{nameof(Color)}.{nameof(HullPrimaryColor)}");
            public static readonly Resource<Color> HullSecondaryColor = Resource.Get<Color>($"{nameof(Color)}.{nameof(HullSecondaryColor)}");

            public static readonly Resource<Color> ThrusterPrimaryColor = Resource.Get<Color>($"{nameof(Color)}.{nameof(ThrusterPrimaryColor)}");
            public static readonly Resource<Color> ThrusterSecondaryColor = Resource.Get<Color>($"{nameof(Color)}.{nameof(ThrusterSecondaryColor)}");

            public static readonly Resource<Color> TeamOnePrimaryColor = Resource.Get<Color>($"{nameof(Color)}.{nameof(TeamOnePrimaryColor)}");
            public static readonly Resource<Color> TeamOneSecondaryColor = Resource.Get<Color>($"{nameof(Color)}.{nameof(TeamOneSecondaryColor)}");

            public static readonly Resource<Color> TractorBeamHighlight = Resource.Get<Color>($"{nameof(Color)}.{nameof(TractorBeamHighlight)}");
        }
    }
}

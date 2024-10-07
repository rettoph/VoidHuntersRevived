using Guppy.Core.Resources.Common;
using Guppy.Game.Graphics.Common.Resources;
using Guppy.Game.ImGui.Common.Styling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Game.Core
{
    public static class Resources
    {
        public static class SpriteFonts
        {
            public static readonly Resource<SpriteFont> Default = Resource<SpriteFont>.Get($"{nameof(SpriteFont)}.{nameof(Default)}");
        }

        public static class EffectCodes
        {
            public static readonly Resource<IEffectCode> ShaderAntiAliasing = Resource<IEffectCode>.Get($"EffectCode.{nameof(ShaderAntiAliasing)}");
            public static readonly Resource<IEffectCode> Visible = Resource<IEffectCode>.Get($"EffectCode.{nameof(Visible)}");
        }

        public static class Strings
        {
            public static readonly Resource<string> TeamZeroName = Resource<string>.Get($"{nameof(String)}.{nameof(TeamZeroName)}");
            public static readonly Resource<string> TeamOneName = Resource<string>.Get($"{nameof(String)}.{nameof(TeamOneName)}");
        }

        public static class Colors
        {
            public static readonly Resource<Color> None = Resource<Color>.Get($"{nameof(Color)}.{nameof(None)}");

            public static readonly Resource<Color> HullPrimaryColor = Resource<Color>.Get($"{nameof(Color)}.{nameof(HullPrimaryColor)}");
            public static readonly Resource<Color> HullSecondaryColor = Resource<Color>.Get($"{nameof(Color)}.{nameof(HullSecondaryColor)}");

            public static readonly Resource<Color> ThrusterPrimaryColor = Resource<Color>.Get($"{nameof(Color)}.{nameof(ThrusterPrimaryColor)}");
            public static readonly Resource<Color> ThrusterSecondaryColor = Resource<Color>.Get($"{nameof(Color)}.{nameof(ThrusterSecondaryColor)}");

            public static readonly Resource<Color> TeamOnePrimaryColor = Resource<Color>.Get($"{nameof(Color)}.{nameof(TeamOnePrimaryColor)}");
            public static readonly Resource<Color> TeamOneSecondaryColor = Resource<Color>.Get($"{nameof(Color)}.{nameof(TeamOneSecondaryColor)}");

            public static readonly Resource<Color> TractorBeamHighlight = Resource<Color>.Get($"{nameof(Color)}.{nameof(TractorBeamHighlight)}");
            public static readonly Resource<Color> ActiveThrustableHighlight = Resource<Color>.Get($"{nameof(Color)}.{nameof(TractorBeamHighlight)}");
        }

        public static class ImGuiStyles
        {
            public static readonly Resource<ImStyle> ButtonGreen = Resource<ImStyle>.Get($"{nameof(ImStyle)}.{nameof(ButtonGreen)}");
            public static readonly Resource<ImStyle> ButtonRed = Resource<ImStyle>.Get($"{nameof(ImStyle)}.{nameof(ButtonRed)}");
        }
    }
}

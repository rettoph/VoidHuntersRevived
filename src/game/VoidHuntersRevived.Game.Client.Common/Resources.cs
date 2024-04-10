using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Game.Client.Common
{
    public static class Resources
    {
        public static class SpriteFonts
        {
            public static readonly Resource<SpriteFont> Default = Resource<SpriteFont>.Get($"{nameof(SpriteFont)}.{nameof(Default)}");
        }

        public static class EffectCodes
        {
            public static readonly Resource<EffectCode> ShaderAntiAliasing = Resource<EffectCode>.Get($"{nameof(EffectCode)}.{nameof(ShaderAntiAliasing)}");
            public static readonly Resource<EffectCode> VisibleAccum = Resource<EffectCode>.Get($"{nameof(EffectCode)}.{nameof(VisibleAccum)}");
            public static readonly Resource<EffectCode> VisibleFinal = Resource<EffectCode>.Get($"{nameof(EffectCode)}.{nameof(VisibleFinal)}");
        }
    }
}

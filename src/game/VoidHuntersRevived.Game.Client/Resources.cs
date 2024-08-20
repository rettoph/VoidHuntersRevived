using Guppy.Core.Resources.Common;
using Guppy.Game.MonoGame.Common.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Game.Client
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
            public static readonly Resource<EffectCode> Visible = Resource<EffectCode>.Get($"{nameof(EffectCode)}.{nameof(Visible)}");
        }
    }
}

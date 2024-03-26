using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Game.Client.Common
{
    public static class Resources
    {
        public static class SpriteFonts
        {
            public static readonly Resource<SpriteFont> Default = Resource.Get<SpriteFont>($"{nameof(SpriteFont)}.{nameof(Default)}");
        }

        public static class EffectCodes
        {
            public static readonly Resource<EffectCode> VisibleAccum = Resource.Get<EffectCode>($"{nameof(EffectCode)}.{nameof(VisibleAccum)}");
            public static readonly Resource<EffectCode> VisibleFinal = Resource.Get<EffectCode>($"{nameof(EffectCode)}.{nameof(VisibleFinal)}");
        }
    }
}

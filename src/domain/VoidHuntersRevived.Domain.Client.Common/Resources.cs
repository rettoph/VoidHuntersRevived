using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Client.Common
{
    public static class Resources
    {
        public static class SpriteFonts
        {
            public static readonly Resource<SpriteFont> Default = Resource.Get<SpriteFont>($"{nameof(SpriteFont)}.{nameof(Default)}");
        }

        public static class EffectCodes
        {
            public static readonly Resource<EffectCode> Visible = Resource.Get<EffectCode>($"{nameof(EffectCode)}.{nameof(Visible)}");
        }
    }
}

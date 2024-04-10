using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    internal class VisibleFinalEffect : Effect
    {
        public Matrix WorldViewProjection
        {
            set => this.Parameters[nameof(WorldViewProjection)].SetValue(value);
        }

        public bool HideTop
        {
            set => this.Parameters[nameof(HideTop)].SetValue(value);
        }

        public bool HideAccum
        {
            set => this.Parameters[nameof(HideAccum)].SetValue(value);
        }

        public Texture2D AccumTexture
        {
            set => this.Parameters[nameof(AccumTexture)].SetValue(value);
        }

        public VisibleFinalEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice, Resources.EffectCodes.VisibleFinal.Value)
        {
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    internal class VisibleAccumEffect : Effect
    {
        public Matrix WorldViewProjection
        {
            set => this.Parameters[nameof(WorldViewProjection)].SetValue(value);
        }

        public VisibleAccumEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice, Resources.EffectCodes.VisibleAccum.Value)
        {
        }
    }
}

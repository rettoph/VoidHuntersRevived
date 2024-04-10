using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    internal class ShaderAntiAliasingEffect : Effect
    {
        public Vector2 Pixel
        {
            set => this.Parameters[nameof(Pixel)].SetValue(value);
        }

        public ShaderAntiAliasingEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice, Resources.EffectCodes.ShaderAntiAliasing.Value)
        {
        }
    }
}

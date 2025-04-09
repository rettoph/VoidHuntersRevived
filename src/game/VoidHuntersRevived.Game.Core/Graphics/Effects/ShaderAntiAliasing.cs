using Guppy.Core.Assets.Common.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Common;

namespace VoidHuntersRevived.Game.Core.Graphics.Effects
{
    public class ShaderAntiAliasingEffect(GraphicsDevice graphicsDevice, IAssetService resources) : Effect(graphicsDevice, resources.Get(Assets.EffectCodes.ShaderAntiAliasing).Value.Data)
    {
        public Vector2 Pixel
        {
            set => this.Parameters[nameof(this.Pixel)].SetValue(value);
        }
    }
}
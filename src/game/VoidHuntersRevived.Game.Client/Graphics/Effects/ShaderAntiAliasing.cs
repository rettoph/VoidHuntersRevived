using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    internal class ShaderAntiAliasingEffect : Effect
    {
        public Vector2 Pixel
        {
            set => this.Parameters[nameof(Pixel)].SetValue(value);
        }

        public ShaderAntiAliasingEffect(GraphicsDevice graphicsDevice, IResourceService resources) : base(graphicsDevice, resources.GetValue(Resources.EffectCodes.ShaderAntiAliasing).Value)
        {
        }
    }
}

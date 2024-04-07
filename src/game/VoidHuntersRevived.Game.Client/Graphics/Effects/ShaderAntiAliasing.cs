using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources.Providers;
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

        public ShaderAntiAliasingEffect(GraphicsDevice graphicsDevice, IResourceProvider resources) : base(graphicsDevice, resources.Get<EffectCode>(Resources.EffectCodes.ShaderAntiAliasing).Value)
        {
        }
    }
}

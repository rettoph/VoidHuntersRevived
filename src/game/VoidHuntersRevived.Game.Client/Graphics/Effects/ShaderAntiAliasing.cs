using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    internal class ShaderAntiAliasingEffect : Effect
    {
        public ShaderAntiAliasingEffect(GraphicsDevice graphicsDevice, IResourceProvider resources) : base(graphicsDevice, resources.Get<EffectCode>(Resources.EffectCodes.ShaderAntiAliasing).Value)
        {
        }
    }
}

using Guppy.Core.Resources.Common.Services;
using Guppy.Game.Graphics.Common.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Game.Core.Graphics.Effects
{
    public class VisibleEffect(
        GraphicsDevice graphicsDevice,
        IResourceService resourceService
    ) : Effect(
        graphicsDevice,
        resourceService.GetValue(Resources.EffectCodes.Visible).Value.Data),
            IWorldViewProjectionEffect
    {
        public Matrix WorldViewProjection
        {
            set => this.Parameters[nameof(WorldViewProjection)].SetValue(value);
        }
    }
}

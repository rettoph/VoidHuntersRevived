using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Graphics.Common.Effects;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    public class VisibleEffect(
        GraphicsDevice graphicsDevice,
        IResourceService resourceService
    ) : Effect(
        graphicsDevice,
        resourceService.GetValue(Resources.EffectCodes.Visible).Value),
            IWorldViewProjectionEffect
    {
        public Matrix WorldViewProjection
        {
            set => this.Parameters[nameof(WorldViewProjection)].SetValue(value);
        }
    }
}

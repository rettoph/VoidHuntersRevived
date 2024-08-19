using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    public class VisibleEffect : Effect
    {
        public Matrix WorldViewProjection
        {
            set => this.Parameters[nameof(WorldViewProjection)].SetValue(value);
        }

        public VisibleEffect(GraphicsDevice graphicsDevice, IResourceService resourceService) : base(graphicsDevice, resourceService.GetValue(Resources.EffectCodes.Visible).Value)
        {
        }
    }
}

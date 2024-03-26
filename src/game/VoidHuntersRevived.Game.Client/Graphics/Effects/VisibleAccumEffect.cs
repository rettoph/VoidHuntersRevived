using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources.Providers;
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

        public float TraceScale
        {
            set => this.Parameters[nameof(TraceScale)].SetValue(value);
        }

        public float TraceDiffusionScale
        {
            set => this.Parameters[nameof(TraceDiffusionScale)].SetValue(value);
        }

        public VisibleAccumEffect(GraphicsDevice graphicsDevice, IResourceProvider resources) : base(graphicsDevice, resources.Get<EffectCode>(Resources.EffectCodes.VisibleAccum).Value)
        {
        }
    }
}

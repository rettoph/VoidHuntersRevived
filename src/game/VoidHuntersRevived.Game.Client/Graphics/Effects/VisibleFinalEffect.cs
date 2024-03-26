using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Game.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    internal class VisibleFinalEffect : Effect
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

        public Texture2D AccumTexture
        {
            set => this.Parameters[nameof(AccumTexture)].SetValue(value);
        }

        public VisibleFinalEffect(GraphicsDevice graphicsDevice, IResourceProvider resources) : base(graphicsDevice, resources.Get<EffectCode>(Resources.EffectCodes.VisibleFinal).Value)
        {
        }
    }
}

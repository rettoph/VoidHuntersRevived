using Guppy.Attributes;
using Guppy.Game.MonoGame.Graphics.Effects;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    [AutoLoad]
    public class VisibleEffect : EffectMatricesEffect
    {
        private EffectParameter _primaryColor;
        private EffectParameter _secondaryColor;
        private EffectParameter _traceScale;
        private EffectParameter _traceDiffustionScale;

        public float TraceScale
        {
            set => _traceScale.SetValue(value);
        }

        public float TraceDiffusionScale
        {
            set => _traceDiffustionScale.SetValue(value);
        }

        public VisibleEffect(GraphicsDevice graphics, IResourceProvider resources) : base(graphics, resources.Get(Resources.EffectCodes.Visible).Value)
        {
            _traceScale = this.Parameters[nameof(TraceScale)];
            _traceDiffustionScale = this.Parameters[nameof(TraceDiffusionScale)];
        }
    }
}

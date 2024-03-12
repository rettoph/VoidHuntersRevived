using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Client.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    public class VisibleEffect : Effect
    {
        private EffectParameter _view;
        private EffectParameter _projection;
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

        public Matrix View
        {
            set => _view.SetValue(value);
        }

        public Matrix Projection
        {
            set => _projection.SetValue(value);
        }

        public VisibleEffect(GraphicsDevice graphics, IResourceProvider resources) : base(graphics, resources.Get(Resources.EffectCodes.Visible).Value)
        {
            _view = this.Parameters[nameof(View)];
            _projection = this.Parameters[nameof(Projection)];

            _traceScale = this.Parameters[nameof(TraceScale)];
            _traceDiffustionScale = this.Parameters[nameof(TraceDiffusionScale)];
        }
    }
}

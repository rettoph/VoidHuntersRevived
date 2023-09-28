using Guppy.Attributes;
using Guppy.MonoGame.Graphics.Effects;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Client.Graphics.Effects
{
    [AutoLoad]
    public class VisibleEffect : EffectMatricesEffect
    {
        private EffectParameter _primaryColor;
        private EffectParameter _secondaryColor;
        private EffectParameter _traceScale;
        private EffectParameter _traceDiffustionScale;

        public Color PrimaryColor
        {
            set => _primaryColor.SetValue(value.ToVector4());
        }

        public Color SecondaryColor
        {
            set => _secondaryColor.SetValue(value.ToVector4());
        }

        public float TraceScale
        {
            set => _traceScale.SetValue(value);
        }

        public float TraceDiffusionScale
        {
            set => _traceDiffustionScale.SetValue(value);
        }

        public VisibleEffect(GraphicsDevice graphics, IResourceProvider resources) : base(graphics, resources.Get(Resources.EffectCodes.Visible))
        {
            _primaryColor = this.Parameters[nameof(PrimaryColor)];
            _secondaryColor = this.Parameters[nameof(SecondaryColor)];
            _traceScale = this.Parameters[nameof(TraceScale)];
            _traceDiffustionScale = this.Parameters[nameof(TraceDiffusionScale)];
        }
    }
}

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
        private EffectParameter _zoom;

        public Color PrimaryColor
        {
            set => _primaryColor.SetValue(value.ToVector4());
        }

        public Color SecondaryColor
        {
            set => _secondaryColor.SetValue(value.ToVector4());
        }

        public float Zoom
        {
            set => _zoom.SetValue(value);
        }

        public VisibleEffect(GraphicsDevice graphics, IResourceProvider resources) : base(graphics, resources.Get(Resources.EffectCodes.Visible))
        {
            _primaryColor = this.Parameters["PrimaryColor"];
            _secondaryColor = this.Parameters["SecondaryColor"];
            _zoom = this.Parameters["Zoom"];
        }
    }
}

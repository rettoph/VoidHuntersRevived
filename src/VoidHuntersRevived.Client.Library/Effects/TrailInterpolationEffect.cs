using Guppy.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Effects
{
    public class TrailInterpolationEffect : EffectMatricesEffect
    {
        #region Static Fields
        private static Byte[] FileResourceBytes = EffectHelper.GetFileResourceBytes("Content/Effects/Compiled/TrailInterpolation.mgfx");
        #endregion

        #region Private Fields
        private EffectParameter _currentTimestamp;
        private EffectParameter _maxAge;
        private EffectParameter _spreadSpeed;
        #endregion

        #region Public Properties
        public Single CurrentTimestamp
        {
            set => _currentTimestamp.SetValue(value);
        }

        public Single MaxAge
        {
            set => _maxAge.SetValue(value);
        }

        public Single SpreadSpeed
        {
            set => _spreadSpeed.SetValue(value);
        }
        #endregion

        public TrailInterpolationEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice, TrailInterpolationEffect.FileResourceBytes)
        {
            _currentTimestamp = this.Parameters["CurrentTimestamp"];
            _maxAge = this.Parameters["MaxAge"];
            _spreadSpeed = this.Parameters["SpreadSpeed"];
        }
    }
}

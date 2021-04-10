using Guppy.Effects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Graphics.Effects
{
    public class TrailEffect : EffectMatricesEffect
    {
        #region Static Fields
        private static Byte[] FileResourceBytes = EffectHelper.GetFileResourceBytes("Content/Effects/Compiled/Trail.mgfx");
        #endregion

        #region Private Fields
        private EffectParameter _currentTimestamp;
        #endregion

        #region Public Properties
        public Single CurrentTimestamp
        {
            set => _currentTimestamp.SetValue(value);
        }
        #endregion

        #region Constructor
        public TrailEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice, TrailEffect.FileResourceBytes)
        {
            _currentTimestamp = this.Parameters["CurrentTimestamp"];
        }
        #endregion
    }
}

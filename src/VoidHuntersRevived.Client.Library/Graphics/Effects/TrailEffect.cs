using Guppy.Effects;
using Guppy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

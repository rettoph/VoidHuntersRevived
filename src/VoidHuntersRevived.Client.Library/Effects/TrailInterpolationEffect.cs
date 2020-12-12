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

        public TrailInterpolationEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice, FileResourceBytes)
        {
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public class RadianInput : SingleInput
    {
        #region Public Properties
        public override Single Value
        {
            get => MathHelper.ToRadians(base.Value);
            set => base.Value = MathHelper.ToDegrees(value);
        }
        #endregion
    }
}

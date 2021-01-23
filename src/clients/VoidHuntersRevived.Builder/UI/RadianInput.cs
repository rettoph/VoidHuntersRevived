using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.UI
{
    public class RadianInput : SingleInput
    {
        public override Single Value { 
            get => MathHelper.ToRadians(base.Value); 
            set => base.Value = MathHelper.ToDegrees(value); 
        }
    }
}

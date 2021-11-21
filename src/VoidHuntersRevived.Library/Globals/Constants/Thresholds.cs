using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class Thresholds
    {
        public const Single SlaveBodyPositionSnapThreshold = 5f;
        public const Single SlaveBodyRotationSnapThreshold = MathHelper.PiOver2;
        public const Single SlaveBodyPositionDifferenceTheshold = 0.001f;
        public const Single SlaveBodyRotationDifferenceTheshold = 0.0001f;

        public const Single MasterBodyAngularVelocityDifferenceTheshold = 0.001f;
        public const Single MasterBodyLinearVelocityDifferenceTheshold = 0.0001f;
    }
}

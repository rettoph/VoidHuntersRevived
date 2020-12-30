using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace VoidHuntersRevived.Library.Extensions.Aether
{
    public static class JointExtensions
    {
        #region Remove Methods
        public static void TryRemove(this Joint joint)
            => joint?.BodyA?.World?.Remove(joint);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace VoidHuntersRevived.Library.Extensions.Aether
{
    public static class FixtureExtensions
    {
        #region Remove Methods
        public static void Remove(this Fixture fixture)
            => fixture.Body.Remove(fixture);
        #endregion
    }
}

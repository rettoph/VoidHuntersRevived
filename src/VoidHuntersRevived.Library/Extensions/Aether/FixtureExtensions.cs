using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Library.Extensions.Aether
{
    public static class FixtureExtensions
    {
        #region Remove Methods
        public static void TryRemove(this Fixture fixture)
        {
            if (fixture?.Body?.World == default)
                return;

            fixture.Body.Remove(fixture);
        }
        #endregion
    }
}

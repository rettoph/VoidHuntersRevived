using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.Farseer
{
    public static class BodyExtensions
    {
        #region Transform Methods
        public static void SetTransformIgnoreContacts(this Body body, Vector2 position, Single rotation)
        {
            body.SetTransformIgnoreContacts(ref position, rotation);
        }
        #endregion
    }
}

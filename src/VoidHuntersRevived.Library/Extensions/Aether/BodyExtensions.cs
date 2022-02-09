using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Library.Extensions.Aether
{
    public static class BodyExtensions
    {
        #region Remove Methods
        public static void TryRemove(this Body body)
        {
            if(body?.World == default)
                return;

            body.World.Remove(body);
        }
        #endregion

        #region Transform Methods
        public static void SetTransformIgnoreContacts(this Body body, Vector2 position, Single angle)
        {
            body.SetTransformIgnoreContacts(ref position, angle);
        }

        public static void CopyPosition(this Body body, Body target)
        {
            body.SetTransformIgnoreContacts(target.Position, target.Rotation);
            body.LinearVelocity = target.LinearVelocity;
            body.AngularVelocity = target.AngularVelocity;
        }
        #endregion
    }
}

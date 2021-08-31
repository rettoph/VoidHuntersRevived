using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
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
            => body?.World?.Remove(body);
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

        #region Network Methods
        public static void WriteWorldInfo(this Body body, NetOutgoingMessage om)
        {
            om.Write(body.Position);
            om.Write(body.Rotation);
            om.Write(body.LinearVelocity);
            om.Write(body.AngularVelocity);
        }

        public static void ReadWorldInfo(this Body body, NetIncomingMessage im)
        {
            body.SetTransformIgnoreContacts(im.ReadVector2(), im.ReadSingle());

            body.LinearVelocity = im.ReadVector2();
            body.AngularVelocity = im.ReadSingle();
        }
        #endregion
    }
}

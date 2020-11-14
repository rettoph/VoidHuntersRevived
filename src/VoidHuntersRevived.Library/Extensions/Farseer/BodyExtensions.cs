using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Extensions.Farseer
{
    public static class BodyExtensions
    {
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
        public static void WritePosition(this Body body, NetOutgoingMessage om)
        {
            om.Write(body.Position);
            om.Write(body.Rotation);
            om.Write(body.LinearVelocity);
            om.Write(body.AngularVelocity);
        }

        public static void ReadPosition(this Body body, NetIncomingMessage im)
        {
            body.SetTransform(im.ReadVector2(), im.ReadSingle());

            body.LinearVelocity = im.ReadVector2();
            body.AngularVelocity = im.ReadSingle();
        }
        #endregion
    }
}

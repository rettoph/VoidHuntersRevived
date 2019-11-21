using FarseerPhysics.Dynamics;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region EnabledSolid Methods
        public static Boolean IsSolidEnabled(this Body body)
        {
            return body.Enabled && body.FixtureList.Any();
        }
        #endregion

        #region Network Methods
        public static void WriteVitals(this Body body, NetOutgoingMessage om)
        {
            om.Write(body.Position);
            om.Write(body.Rotation);
            om.Write(body.LinearVelocity);
            om.Write(body.AngularVelocity);
        }

        public static void ReadVitals(this Body body, NetIncomingMessage im)
        {
            body.SetTransformIgnoreContacts(im.ReadVector2(), im.ReadSingle());
            body.LinearVelocity = im.ReadVector2();
            body.AngularVelocity = im.ReadSingle();
        }
        #endregion
    }
}

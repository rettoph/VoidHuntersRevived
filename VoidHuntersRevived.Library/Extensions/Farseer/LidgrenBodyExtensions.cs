using FarseerPhysics.Dynamics;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.Farseer
{
    /// <summary>
    /// Contains farseer Body methods specific to ligren
    /// reading and writing.
    /// </summary>
    public static class LidgrenBodyExtensions
    {
        #region Position Methods
        public static void WritePosition(this Body body, NetOutgoingMessage om)
        {
            om.Write(body.Position);
            om.Write(body.Rotation);
        }

        public static void ReadPosition(this Body body, NetIncomingMessage im)
        {
            body.SetTransformIgnoreContacts(im.ReadVector2(), im.ReadSingle());
        }
        #endregion

        #region Velocity Methods
        public static void WriteVelocity(this Body body, NetOutgoingMessage om)
        {
            om.Write(body.LinearVelocity);
            om.Write(body.AngularVelocity);
        }

        public static void ReadVelocity(this Body body, NetIncomingMessage im)
        {
            body.LinearVelocity = im.ReadVector2();
            body.AngularVelocity = im.ReadSingle();
        }
        #endregion

        #region Transform Methods
        public static void SetTransformIgnoreContacts(this Body body, Vector2 position, Single rotation)
        {
            body.SetTransformIgnoreContacts(ref position, rotation);
        }
        #endregion
    }
}

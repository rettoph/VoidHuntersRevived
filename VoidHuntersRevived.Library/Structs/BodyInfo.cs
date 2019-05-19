using FarseerPhysics.Dynamics;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Structs
{
    public struct BodyInfo : INetworkObject
    {
        public Vector2 Position;
        public Single Rotation;
        public Vector2 LinearVelocity;
        public Single AngularVelocity;

        public BodyInfo(Body body)
        {
            this.Position = body.Position;
            this.Rotation = body.Rotation;
            this.LinearVelocity = body.LinearVelocity;
            this.AngularVelocity = body.AngularVelocity;
        }

        /// <summary>
        /// Step the current body info data forward
        /// by the recieved amount.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            var delta = (Single)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            this.Position += this.LinearVelocity * delta;
            this.Rotation += this.AngularVelocity * delta;
        }

        /// <summary>
        /// Import body data as is
        /// </summary>
        /// <param name="body"></param>
        public void Import(Body body)
        {
            this.Position = body.Position;
            this.Rotation = body.Rotation;
            this.LinearVelocity = body.LinearVelocity;
            this.AngularVelocity = body.AngularVelocity;
        }

        public void Read(NetIncomingMessage im)
        {
            im.ReadVector2(ref this.Position);
            this.Rotation = im.ReadSingle();

            im.ReadVector2(ref this.LinearVelocity);
            this.AngularVelocity = im.ReadSingle();
        }

        public void Write(NetOutgoingMessage om)
        {
            om.Write(this.Position);
            om.Write(this.Rotation);
            om.Write(this.LinearVelocity);
            om.Write(this.AngularVelocity);
        }
    }
}

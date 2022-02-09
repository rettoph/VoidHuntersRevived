using Guppy.Network;
using Guppy.Network.Messages;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages.Network
{
    public sealed class ShipTargetMessage : NetworkEntityMessage<ShipTargetMessage>
    {
        public Vector2 Value { get; set; }

        protected override void Read(NetDataReader im, NetworkProvider network)
        {
            base.Read(im, network);

            this.Value = im.GetVector2();
        }

        protected override void Write(NetDataWriter om, NetworkProvider network)
        {
            base.Write(om, network);

            om.Put(this.Value);
        }
    }
}

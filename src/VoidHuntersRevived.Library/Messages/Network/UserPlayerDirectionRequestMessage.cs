using Guppy.Network;
using Guppy.Network.Messages;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Messages.Network
{
    public class UserPlayerDirectionRequestMessage : NetworkEntityMessage<UserPlayerDirectionRequestMessage>
    {
        public Direction Direction { get; set; }
        public Boolean State { get; set; }

        protected override void Read(NetDataReader im, NetworkProvider network)
        {
            base.Read(im, network);

            this.Direction = im.GetEnum<Direction>();
            this.State = im.GetBool();
        }

        protected override void Write(NetDataWriter om, NetworkProvider network)
        {
            base.Write(om, network);

            om.Put(this.Direction);
            om.Put(this.State);
        }
    }
}

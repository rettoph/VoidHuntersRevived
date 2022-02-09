using Guppy.Network;
using Guppy.Network.Messages;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Network.Packets;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Messages.Network
{
    public class UserPlayerTractorBeamStateRequestMessage : NetworkEntityMessage<UserPlayerTractorBeamStateRequestMessage>
    {
        /// <summary>
        /// The <see cref="TractorBeamStateType"/> this current <see cref="Action"/>
        /// is defining.
        /// </summary>
        public TractorBeamStateType Type { get; set; }

        /// <summary>
        /// The target<see cref="ShipPart"/> in question this
        /// <see cref="TractorBeamStateType"/> is to be preformed on.
        /// </summary>
        public UInt16? TargetPartNetworkId { get; set; }

        /// <summary>
        /// The <see cref="ConnectionNode"/>'s owner, if any, this <see cref="TractorBeamStateType"/> is to be
        /// preformed on. This is generally used to defined which node
        /// the <see cref="TargetPart"/> wishes to attach to when
        /// the <see cref="Type"/> is <see cref="TractorBeamStateType.Deselect"/>.
        /// </summary>
        public ConnectionNodeNetworkId? DestinationNodeNetworkId { get; set; }

        protected override void Read(NetDataReader im, NetworkProvider network)
        {
            base.Read(im, network);

            this.Type = im.GetEnum<TractorBeamStateType>();
            this.DestinationNodeNetworkId = im.GetConnectionNodeNetworkdId();

            if (im.GetIf())
            {
                this.TargetPartNetworkId = im.GetUShort();
            }
        }

        protected override void Write(NetDataWriter om, NetworkProvider network)
        {
            base.Write(om, network);

            om.Put(this.Type);
            om.Put(this.DestinationNodeNetworkId);

            if (om.PutIf(this.TargetPartNetworkId is not null))
            {
                om.Put(this.TargetPartNetworkId.Value);
            }
        }
    }
}

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
    public class WorldObjectPositionPing : NetworkEntityMessage<WorldObjectPositionPing>
    {
        public UInt32 Index { get; set; }

        #region Read/Write Methods
        protected override void Read(NetDataReader im, NetworkProvider network)
        {
            base.Read(im, network);

            this.Index = im.GetUInt();
        }

        protected override void Write(NetDataWriter om, NetworkProvider network)
        {
            base.Write(om, network);

            om.Put(this.Index);
        }
        #endregion
    }
}

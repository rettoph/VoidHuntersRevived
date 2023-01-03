using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations.Events;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class UserJoinedNetSerializer : NetSerializer<UserJoined>
    {
        public override UserJoined Deserialize(NetDataReader reader)
        {
            return new UserJoined();
        }

        public override void Serialize(NetDataWriter writer, in UserJoined instance)
        {
        }
    }
}

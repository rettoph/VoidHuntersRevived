using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Messages;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Serializers.NetSerializers
{
    [AutoLoad]
    internal sealed class UserPilotNetSerializer : NetSerializer<UserPilot>
    {
        public override UserPilot Deserialize(NetDataReader reader, INetSerializerProvider serializers)
        {
            var user = serializers.Deserialize<UserAction>(reader);

            return new UserPilot(user);
        }

        public override void Serialize(NetDataWriter writer, INetSerializerProvider serializers, in UserPilot instance)
        {
            serializers.Serialize(writer, false, instance.User);
        }
    }
}

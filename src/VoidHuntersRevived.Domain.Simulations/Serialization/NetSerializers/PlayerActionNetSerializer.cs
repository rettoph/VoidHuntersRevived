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
using VoidHuntersRevived.Domain.Simulations.Events;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class PlayerActionNetSerializer : NetSerializer<PlayerAction>
    {
        private INetSerializer<UserAction> _serializer = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializer = serializers.Get<UserAction>();
        }

        public override PlayerAction Deserialize(NetDataReader reader)
        {
            return new PlayerAction()
            {
                UserAction = _serializer.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in PlayerAction instance)
        {
            _serializer.Serialize(writer, instance.UserAction);
        }
    }
}

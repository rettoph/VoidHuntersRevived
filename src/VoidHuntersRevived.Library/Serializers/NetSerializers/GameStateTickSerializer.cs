using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Definitions;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Serializers.NetSerializers
{
    [AutoLoad]
    internal sealed class GameStateTickNetSerializer : NetSerializer<GameStateTick>
    {
        private INetSerializer<Tick> _serializer = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializer = serializers.Get<Tick>();
        }

        public override GameStateTick Deserialize(NetDataReader reader)
        {
            return new GameStateTick(_serializer.Deserialize(reader));
        }

        public override void Serialize(NetDataWriter writer, in GameStateTick instance)
        {
            _serializer.Serialize(writer, in instance.Tick);
        }
    }
}

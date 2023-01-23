using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class UserInputNetSerializer : NetSerializer<UserInput>
    {
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override UserInput Deserialize(NetDataReader reader)
        {
            return new UserInput(
                user: reader.GetParallelKey(),
                data: (IData)_serializers.Deserialize(reader));
        }

        public override void Serialize(NetDataWriter writer, in UserInput instance)
        {
            writer.Put(instance.User);
            _serializers.Serialize(writer, instance.Data);
        }
    }
}

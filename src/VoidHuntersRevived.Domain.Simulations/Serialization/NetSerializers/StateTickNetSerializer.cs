using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class StateTickNetSerializer : NetSerializer<StateTick>
    {
        private INetSerializer<Tick> _tickSerializer = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _tickSerializer = serializers.Get<Tick>();
        }

        public override StateTick Deserialize(NetDataReader reader)
        {
            return new StateTick()
            {
                Tick = _tickSerializer.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in StateTick instance)
        {
            _tickSerializer.Serialize(writer, instance.Tick);
        }
    }
}

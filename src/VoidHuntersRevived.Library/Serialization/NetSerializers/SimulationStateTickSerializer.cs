﻿using Guppy.Attributes;
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
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad(0)]
    internal sealed class SimulationStateTickNetSerializer : NetSerializer<SimulationStateTick>
    {
        private INetSerializer<Tick> _serializer = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializer = serializers.Get<Tick>();
        }

        public override SimulationStateTick Deserialize(NetDataReader reader)
        {
            return new SimulationStateTick(_serializer.Deserialize(reader));
        }

        public override void Serialize(NetDataWriter writer, in SimulationStateTick instance)
        {
            _serializer.Serialize(writer, in instance.Tick);
        }
    }
}

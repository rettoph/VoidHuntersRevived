﻿using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Serialization.NetSerializers
{
    [AutoLoad]
    internal class TractorBeamEmitter_Deactivate_NetSerializer : NetSerializer<TractorBeamEmitter_TryDeactivate>
    {
        public override TractorBeamEmitter_TryDeactivate Deserialize(NetDataReader reader)
        {
            return new TractorBeamEmitter_TryDeactivate()
            {
                ShipVhId = reader.GetVhId()
            };
        }

        public override void Serialize(NetDataWriter writer, in TractorBeamEmitter_TryDeactivate instance)
        {
            writer.Put(instance.ShipVhId);
        }
    }
}

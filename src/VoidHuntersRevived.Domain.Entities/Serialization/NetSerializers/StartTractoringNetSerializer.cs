using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class StartTractoringNetSerializer : NetSerializer<TryStartTractoring>
    {
        public override TryStartTractoring Deserialize(NetDataReader reader)
        {
            return new TryStartTractoring()
            {
                EmitterKey = reader.GetParallelKey()
            };
        }

        public override void Serialize(NetDataWriter writer, in TryStartTractoring instance)
        {
            writer.Put(instance.EmitterKey);
        }
    }
}

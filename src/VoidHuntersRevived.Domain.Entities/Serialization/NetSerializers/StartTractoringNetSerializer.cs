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
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class StartTractoringNetSerializer : NetSerializer<StartTractoring>
    {
        public override StartTractoring Deserialize(NetDataReader reader)
        {
            return new StartTractoring()
            {
                Tractorable = reader.GetParallelKey(),
                Node = reader.GetParallelKey()
            };
        }

        public override void Serialize(NetDataWriter writer, in StartTractoring instance)
        {
            writer.Put(instance.Tractorable);
            writer.Put(instance.Node);
        }
    }
}

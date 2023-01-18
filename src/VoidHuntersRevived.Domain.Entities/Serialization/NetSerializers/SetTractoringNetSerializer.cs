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
    internal sealed class SetTractoringNetSerializer : NetSerializer<SetTractoring>
    {
        public override SetTractoring Deserialize(NetDataReader reader)
        {
            return new SetTractoring()
            {
                PilotKey = ParallelKey.From(ParallelTypes.Pilot, reader.GetInt()),
                Value = reader.GetBool()
            };
        }

        public override void Serialize(NetDataWriter writer, in SetTractoring instance)
        {
            writer.Put(instance.PilotKey.Value);
            writer.Put(instance.Value);
        }
    }
}

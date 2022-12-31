using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library
{
    public struct SimulatedId
    {
        public readonly SimulatedIdType Type;
        public readonly ushort Data;

        public SimulatedId(SimulatedIdType type, ushort data)
        {
            this.Type = type;
            this.Data = data;
        }

        public override bool Equals(object? obj)
        {
            return obj is SimulatedId id && id == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Type, this.Data);
        }

        public static bool operator ==(SimulatedId id1, SimulatedId id2)
        {
            return id1.Type == id2.Type && id1.Data == id2.Data;
        }

        public static bool operator !=(SimulatedId id1, SimulatedId id2)
        {
            return id1.Type != id2.Type || id1.Data != id2.Data;
        }

        public static SimulatedId Pilot(ushort id)
        {
            return new SimulatedId(SimulatedIdType.Pilot, id);
        }
    }
}

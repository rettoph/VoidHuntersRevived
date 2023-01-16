using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Entities.Enums;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class SetPilotingDirection : Message<SetPilotingDirection>, IData
    {
        /// <summary>
        /// When null attempt to load the concept of a "current pilot"
        /// This will only produce valid results on a client instance.
        /// </summary>
        public ParallelKey PilotKey { get; }
        public Direction Which { get; }
        public bool Value { get; }

        public SetPilotingDirection(ParallelKey pilotKey, Direction which, bool value)
        {
            this.PilotKey = pilotKey;
            this.Which = which;
            this.Value = value;
        }

        public override bool Equals(object? obj)
        {
            return obj is SetPilotingDirection direction &&
                   EqualityComparer<ParallelKey>.Default.Equals(PilotKey, direction.PilotKey) &&
                   Which == direction.Which &&
                   Value == direction.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PilotKey, Which, Value);
        }
    }
}

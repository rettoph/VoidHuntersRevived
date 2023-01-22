using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class SetPilotingDirection : Message<SetPilotingDirection>, IData
    {
        public Direction Which { get; }
        public bool Value { get; }

        public SetPilotingDirection(Direction which, bool value)
        {
            this.Which = which;
            this.Value = value;
        }

        public override bool Equals(object? obj)
        {
            return obj is SetPilotingDirection direction &&
                   Which == direction.Which &&
                   Value == direction.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Which, Value);
        }
    }
}

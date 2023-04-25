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
    public class SetPilotingDirection : Input, IMessage
    {
        public required Direction Which { get; init;  }
        public required bool Value { get; init;  }

        public Type Type => typeof(SetPilotingDirection);

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

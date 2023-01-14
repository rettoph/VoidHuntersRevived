using Guppy.Common;
using Guppy.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Events
{
    public class PlayerAction : IData
    {
        public required UserAction UserAction { get; init; }

        public override bool Equals(object? obj)
        {
            return obj is PlayerAction action &&
                   EqualityComparer<UserAction>.Default.Equals(UserAction, action.UserAction);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserAction.Action, UserAction.Id);
        }
    }
}

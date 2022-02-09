using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages
{
    /// <summary>
    /// The base type that can be passed into a ConsolidationProcessor instance.
    /// </summary>
    public class ConsolidableMessage : IMessage
    {
        public Guid Id { get; init; }

        /// <summary>
        /// Determins who created the current request. Useful for knowing whether or not an action performed
        /// by the CurrentUser's tractor beam should be broadcasted to the server or not.
        /// </summary>
        public HostType RequestHost { get; init; }
    }
}

using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class SetTractoring : Message<SetTractoring>, IData
    {
        public required ParallelKey PilotKey { get; init; }
        public required bool Value { get; init; }
    }
}

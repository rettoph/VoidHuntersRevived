using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class StopTractoring : SimulationInput, IMessage
    {
        public static readonly StopTractoring Instance = new StopTractoring()
        {
            Id = ParallelKey.Empty,
            Sender = default!
        };

        public Vector2 TargetPosition { get; init; }
        public ParallelKey TargetTreeKey { get; init; }

        public Type Type => typeof(StopTractoring);
    }
}

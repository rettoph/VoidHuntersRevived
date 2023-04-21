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
    public class StopTractoring : Message<StopTractoring>, IData
    {
        public static readonly StopTractoring Instance = new StopTractoring();

        public Vector2 TargetPosition { get; init; }
        public ParallelKey TractorableKey { get; init; }
    }
}

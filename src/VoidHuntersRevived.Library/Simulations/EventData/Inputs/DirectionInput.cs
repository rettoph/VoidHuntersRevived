using Guppy.Common;
using Guppy.MonoGame;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Simulations.EventData.Inputs
{
    public class DirectionInput : Input, ISimulationEventData
    {
        public Direction Which { get; }
        public bool Value { get; }

        public DirectionInput(Direction which, bool value)
        {
            Which = which;
            Value = value;
        }
    }
}

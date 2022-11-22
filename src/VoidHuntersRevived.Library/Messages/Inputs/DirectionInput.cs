using Guppy.Common;
using Guppy.MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages.Inputs
{
    public class DirectionInput : Message
    {
        public Direction Which { get; }
        public bool Value { get; }

        public DirectionInput(Direction which, bool value)
        {
            this.Which = which;
            this.Value = value;
        }
    }
}

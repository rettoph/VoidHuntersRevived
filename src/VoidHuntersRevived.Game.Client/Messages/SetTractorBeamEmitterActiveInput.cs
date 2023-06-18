using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal sealed class SetTractorBeamEmitterActiveInput : Message<SetTractorBeamEmitterActiveInput>
    {
        public SetTractorBeamEmitterActiveInput(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
    }
}

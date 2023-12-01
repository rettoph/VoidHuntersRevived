using Guppy.Common;
using Guppy.Input;
using Guppy.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Game.Client.Messages
{
    internal sealed class Input_TractorBeamEmitter_SetActive : Message<Input_TractorBeamEmitter_SetActive>, IInput
    {
        public readonly bool Value;

        public Input_TractorBeamEmitter_SetActive(bool value)
        {
            Value = value;
        }
    }
}

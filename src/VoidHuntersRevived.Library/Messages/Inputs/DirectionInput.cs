﻿using Guppy.Common;
using Guppy.MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Messages.Inputs
{
    public class DirectionInput : Message
    {
        public readonly Direction Which;
        public readonly bool Value;

        public DirectionInput(Direction which, bool value)
        {
            this.Which = which;
            this.Value = value;
        }
    }
}

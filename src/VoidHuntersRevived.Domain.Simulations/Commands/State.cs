using Guppy.Attributes;
using Guppy.Commands.Arguments;
using Guppy.Commands.Attributes;
using Guppy.Common;
using Guppy.MonoGame.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Commands
{
    [Command]
    public class State
    {
        [Command(name: "t")]
        public class Ticks : Message<Ticks>
        {
        }
    }
}

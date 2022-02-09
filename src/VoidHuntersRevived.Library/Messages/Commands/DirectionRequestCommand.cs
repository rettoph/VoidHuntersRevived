using Guppy.CommandLine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Messages.Commands
{
    public class DirectionRequestCommand : ICommandData
    {
        public Direction Direction { get; init; }
        public Boolean State { get; init; }
    }
}

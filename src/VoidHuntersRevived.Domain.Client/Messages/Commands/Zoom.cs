using Guppy.Commands.Arguments;
using Guppy.Commands.Attributes;
using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Client.Messages.Commands
{
    [Command]
    public class Zoom : Message<Zoom>
    {
        [Option(names: new[] { "-target", "-t" })]
        public float? Target { get; set; }
    }
}

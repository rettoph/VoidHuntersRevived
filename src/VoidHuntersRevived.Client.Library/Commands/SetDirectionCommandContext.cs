using Guppy.IO.Commands;
using Guppy.IO.Commands.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Commands
{
    internal sealed class SetDirectionCommandContext : ICommandContext
    {
        public string Description { get; } = "Set the local player's ship's direction.";

        public ArgContext[] Arguments { get; } = new ArgContext[] {
            new ArgContext(ArgType.String, "direction", "The target direction to change.", new Char[] { 'd' }, true),
            new ArgContext(ArgType.String, "value", "The value to set the direction to.", new Char[] { 'v' }, true)
        };

        public object GetOutput(Dictionary<string, object> args)
        {
            return args;
        }
    }
}

using Guppy.Game.Common;
using Guppy.Game.Extensions.Serilog;
using Serilog;
using VoidHuntersRevived.Presentation.Core;

namespace VoidHuntersRevived.Presentation.Client
{
    internal sealed class ClientSerilogSinkConfigurator(ITerminal terminal) : ISerilogSinkConfigurator
    {
        private readonly ITerminal _terminal = terminal;

        public void Configure(LoggerConfiguration config, string template)
        {
            config.WriteTo.Terminal(this._terminal, outputTemplate: template);
        }
    }
}
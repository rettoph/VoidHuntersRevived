using Guppy.Game.Common;
using Serilog;
using VoidHuntersRevived.Presentation.Core;

namespace VoidHuntersRevived.Presentation.Client
{
    internal sealed class ServerSerilogSinkConfigurator(ITerminal terminal) : ISerilogSinkConfigurator
    {
        private readonly ITerminal _terminal = terminal;

        public void Configure(LoggerConfiguration config, string template)
        {
            config.WriteTo.Console(outputTemplate: template);
        }
    }
}
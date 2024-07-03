using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Serilog;
using VoidHuntersRevived.Presentation.Core;

namespace VoidHuntersRevived.Presentation.Client
{
    [AutoLoad]
    internal sealed class ServerSerilogSinkConfigurator : ISerilogSinkConfigurator
    {
        private readonly ITerminal _terminal;

        public ServerSerilogSinkConfigurator(ITerminal terminal)
        {
            _terminal = terminal;
        }

        public void Configure(LoggerConfiguration config, string template)
        {
            config.WriteTo.Console(outputTemplate: template);
        }
    }
}

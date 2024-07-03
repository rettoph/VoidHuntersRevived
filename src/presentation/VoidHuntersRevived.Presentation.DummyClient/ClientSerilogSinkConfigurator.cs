using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Guppy.Game.Extensions.Serilog;
using Serilog;
using VoidHuntersRevived.Presentation.Core;

namespace VoidHuntersRevived.Presentation.Client
{
    [AutoLoad]
    internal sealed class ClientSerilogSinkConfigurator : ISerilogSinkConfigurator
    {
        private readonly ITerminal _terminal;

        public ClientSerilogSinkConfigurator(ITerminal terminal)
        {
            _terminal = terminal;
        }

        public void Configure(LoggerConfiguration config, string template)
        {
            config.WriteTo.Terminal(_terminal, outputTemplate: template);
        }
    }
}

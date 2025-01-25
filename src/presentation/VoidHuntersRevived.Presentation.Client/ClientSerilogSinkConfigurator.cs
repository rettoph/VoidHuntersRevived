using Guppy.Game.Common;
using Serilog;
using VoidHuntersRevived.Presentation.Core;

namespace VoidHuntersRevived.Presentation.Client
{
    internal sealed class ClientSerilogSinkConfigurator(ITerminal terminal) : ISerilogSinkConfigurator
    {
        private readonly ITerminal _terminal = terminal;

        public void Configure(LoggerConfiguration config, string template)
        {
            throw new NotImplementedException();
            //config.WriteTo.Terminal(this._terminal, outputTemplate: template);// config.WriteTo.Console(outputTemplate: template);
        }
    }
}
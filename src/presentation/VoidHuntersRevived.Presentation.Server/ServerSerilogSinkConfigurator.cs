using Serilog;
using VoidHuntersRevived.Presentation.Core;

namespace VoidHuntersRevived.Presentation.Client
{
    internal sealed class ServerSerilogSinkConfigurator() : ISerilogSinkConfigurator
    {
        public void Configure(LoggerConfiguration config, string template)
        {
            config.WriteTo.Console(outputTemplate: template);
        }
    }
}
using Serilog;

namespace VoidHuntersRevived.Presentation.Core
{
    public interface ISerilogSinkConfigurator
    {
        void Configure(LoggerConfiguration config, string template);
    }
}
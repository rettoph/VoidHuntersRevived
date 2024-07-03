using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Serilog;

namespace VoidHuntersRevived.Presentation.Core
{
    [Service<ISerilogSinkConfigurator>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public interface ISerilogSinkConfigurator
    {
        void Configure(LoggerConfiguration config, string template);
    }
}

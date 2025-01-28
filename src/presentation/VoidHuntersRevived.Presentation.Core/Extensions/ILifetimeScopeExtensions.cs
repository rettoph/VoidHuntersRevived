using Autofac;
using Guppy.Core.Common;
using VoidHuntersRevived.Presentation.Core.Configurations;

namespace VoidHuntersRevived.Presentation.Core.Extensions
{
    public static class ILifetimeScopeExtensions
    {
        public static string GetLoggerOutputTemplate(this ILifetimeScope scope)
        {
            IConfiguration<LoggerOutputTemplateConfiguration> configuration = scope.Resolve<IConfiguration<LoggerOutputTemplateConfiguration>>();

            return configuration.Value.Value;
        }
    }
}

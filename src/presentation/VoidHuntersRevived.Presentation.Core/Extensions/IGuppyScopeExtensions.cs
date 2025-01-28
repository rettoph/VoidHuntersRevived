using Guppy.Core.Common;
using VoidHuntersRevived.Presentation.Core.Configurations;

namespace VoidHuntersRevived.Presentation.Core.Extensions
{
    public static class IGuppyScopeExtensions
    {
        public static string GetLoggerOutputTemplate(this IGuppyScope scope)
        {
            IConfiguration<LoggerOutputTemplateConfiguration> configuration = scope.Resolve<IConfiguration<LoggerOutputTemplateConfiguration>>();

            return configuration.Value.Value;
        }
    }
}

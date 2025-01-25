using Guppy.Core.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterComponentSerializer<T>(this IGuppyScopeBuilder builder)
            where T : IComponentSerializer
        {
            builder.RegisterType<T>().As<IComponentSerializer>().InstancePerLifetimeScope();

            return builder;
        }
    }
}
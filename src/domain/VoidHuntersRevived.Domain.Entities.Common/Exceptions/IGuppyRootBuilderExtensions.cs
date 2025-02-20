using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterComponentSerializer<T>(this IGuppyRootBuilder builder)
            where T : IComponentSerializer
        {
            builder.RegisterType<T>().As<IComponentSerializer>().InstancePerLifetimeScope();

            return builder;
        }
    }
}
using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Serialization.Json;
using Serilog;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Providers;
using VoidHuntersRevived.Domain.Serialization.Json;

namespace VoidHuntersRevived.Domain.Modules
{
    public sealed class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar<VhId>();
            });

            builder.RegisterType<UniqueNumberProvider>().As<IUniqueNumberProvider>().InstancePerLifetimeScope();

            builder.RegisterType<Fix64Converter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<FixPolarConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<FixVector2Converter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<NativeDynamicArrayCastJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<KeyConverter>().As<JsonConverter>().SingleInstance();

            builder.RegisterInstance<PolymorphicJsonType>(new PolymorphicJsonType<Fix64, object>(nameof(Fix64))).SingleInstance();
        }
    }
}

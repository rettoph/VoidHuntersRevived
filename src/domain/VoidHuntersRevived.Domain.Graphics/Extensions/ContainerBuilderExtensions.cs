using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Json.Converters;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Engines;
using VoidHuntersRevived.Domain.Graphics.Providers;
using VoidHuntersRevived.Domain.Graphics.Serialization.Json;
using VoidHuntersRevived.Domain.Graphics.Services;
using VoidHuntersRevived.Domain.Pieces.ResourceTypes;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Graphics.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDomainGraphicsServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainGraphicsServices), builder =>
            {
                builder.RegisterType<PrimitiveService>().AsImplementedInterfaces().SingleInstance();
                builder.RegisterGeneric(typeof(PrimitiveService<>)).As(typeof(IPrimitiveService<>)).SingleInstance();

                builder.RegisterType<PrimitiveConverter>().As<JsonConverter>().SingleInstance();
                builder.RegisterType<PrimitiveSequenceGroupConverter>().As<JsonConverter>().SingleInstance();
                builder.RegisterType<PrimitiveTypeConverter>().As<JsonConverter>().SingleInstance();
                builder.RegisterType<PolymorphicConverter<IPrimitiveType>>().As<JsonConverter>().SingleInstance();

                builder.RegisterResourceType<PrimitiveTypeResourceType>();

                builder.RegisterType<PrimitiveEntityEngineProvider>().As<IEngineProvider>().InstancePerLifetimeScope();
                builder.RegisterEngine<DrawPrimitivesEngine>();
            });
        }
    }
}

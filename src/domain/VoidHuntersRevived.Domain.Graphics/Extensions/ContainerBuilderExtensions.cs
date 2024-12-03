using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Core.Serialization.Json.Converters;
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

                builder.RegisterJsonConverter<PrimitiveConverter>();
                builder.RegisterJsonConverter<PrimitiveSequenceGroupConverter>();
                builder.RegisterJsonConverter<PrimitiveTypeConverter>();
                builder.RegisterJsonConverter<PolymorphicConverter<IPrimitiveType>>();

                builder.RegisterResourceType<PrimitiveTypeResourceType>();

                builder.RegisterType<PrimitiveEntityEngineProvider>().As<IEngineProvider>().InstancePerLifetimeScope();
                builder.RegisterEngine<DrawPrimitivesEngine>();
            });
        }
    }
}

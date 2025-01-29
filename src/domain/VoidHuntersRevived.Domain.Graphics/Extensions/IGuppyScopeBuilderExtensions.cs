using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Resources.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Core.Serialization.Json.Converters;
using Guppy.Game.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Engines;
using VoidHuntersRevived.Domain.Graphics.Providers;
using VoidHuntersRevived.Domain.Graphics.Serialization.Json;
using VoidHuntersRevived.Domain.Graphics.Services;
using VoidHuntersRevived.Domain.Pieces.ResourceTypes;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Graphics.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterDomainGraphicsServices(this IGuppyScopeBuilder builder)
        {
            builder.RegisterJsonConverter<PrimitiveConverter>();
            builder.RegisterJsonConverter<PrimitiveSequenceGroupConverter>();
            builder.RegisterJsonConverter<PrimitiveTypeConverter>();
            builder.RegisterJsonConverter<PolymorphicConverter<IPrimitiveType>>();

            builder.RegisterResourceType<PrimitiveTypeResourceType>();

            return builder.EnsureRegisteredOnce(nameof(RegisterDomainGraphicsServices), builder =>
            {
                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterType<PrimitiveService>().AsImplementedInterfaces().SingleInstance();
                    builder.RegisterGeneric(typeof(PrimitiveService<>)).As(typeof(IPrimitiveService<>)).SingleInstance();

                    builder.RegisterType<PrimitiveEntityEngineProvider>().As<IEngineProvider>().InstancePerLifetimeScope();
                    builder.RegisterEngine<DrawPrimitivesEngine>();
                });
            });
        }
    }
}
using Autofac;
using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Common.Providers;
using Guppy.Core.Assets.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Core.Serialization.Json.Converters;
using Guppy.Game.Common.Extensions;
using Guppy.Game.Graphics.Common.Extensions;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Providers;
using VoidHuntersRevived.Domain.Graphics.Serialization.Json;
using VoidHuntersRevived.Domain.Graphics.Services;
using VoidHuntersRevived.Domain.Graphics.Systems;
using VoidHuntersRevived.Domain.Pieces.AssetTypes;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Graphics.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterDomainGraphicsServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainGraphicsServices), builder =>
            {
                builder.RegisterJsonConverter<PrimitiveConverter>();
                builder.RegisterJsonConverter<PrimitiveSequenceGroupConverter>();
                builder.RegisterJsonConverter<PrimitiveTypeConverter>();
                builder.RegisterJsonConverter<PolymorphicConverter<IPrimitiveType>>();

                builder.RegisterAssetType<PrimitiveTypeAssetType>();

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterType<PrimitiveService>().AsImplementedInterfaces().SingleInstance();
                    builder.RegisterGeneric(typeof(PrimitiveService<>)).As(typeof(IPrimitiveService<>)).SingleInstance();

                    builder.RegisterGraphicsEnabledFilter(true, builder =>
                    {
                        builder.RegisterSceneSystem<DrawPrimitivesSystem>();
                        builder.RegisterType<PrimitiveEntitySystemProvider>().As<IScopedSystemProvider>().InstancePerLifetimeScope();
                    });
                });
            });
        }
    }
}
using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Extensions;
using VoidHuntersRevived.Domain.Graphics.Extensions;
using VoidHuntersRevived.Domain.Physics.Extensions;
using VoidHuntersRevived.Domain.Pieces.Extensions;
using VoidHuntersRevived.Domain.Providers;
using VoidHuntersRevived.Domain.Serialization.Json;
using VoidHuntersRevived.Domain.Ships.Extensions;
using VoidHuntersRevived.Domain.Simulations.Extensions;
using VoidHuntersRevived.Domain.Teams.Extensions;

namespace VoidHuntersRevived.Domain.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterDomainServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainServices), builder =>
            {
                builder
                    .RegisterDomainCoreServices()
                    .RegisterDomainEntityServices()
                    .RegisterDomainSimulationServices()
                    .RegisterDomainPhysicsServices()
                    .RegisterDomainPiecesServices()
                    .RegisterDomainShipsServices()
                    .RegisterDomainTeamsServices()
                    .RegisterDomainGraphicsServices();
            });
        }

        public static IGuppyRootBuilder RegisterDomainCoreServices(this IGuppyRootBuilder builder)
        {
            builder.RegisterType<UniqueNumberProvider>().As<IUniqueNumberProvider>().InstancePerLifetimeScope();

            builder.RegisterJsonConverter<Fix64Converter>();
            builder.RegisterJsonConverter<FixPolarConverter>();
            builder.RegisterJsonConverter<FixVector2Converter>();
            builder.RegisterJsonConverter<FixTransform2DConverter>();
            builder.RegisterJsonConverter<NativeDynamicArrayCastJsonConverter>();
            builder.RegisterJsonConverter<KeyConverter>();

            builder.RegisterPolymorphicJsonType<Fix64, object>(nameof(Fix64));

            return builder;
        }
    }
}
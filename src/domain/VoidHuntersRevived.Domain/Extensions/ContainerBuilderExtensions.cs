using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Common.Extensions;
using Serilog;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
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
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDomainServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainServices), builder =>
            {
                builder.RegisterDomainEntityServices()
                    .RegisterDomainSimulationServices()
                    .RegisterDomainPhysicsServices()
                    .RegisterDomainPiecesServices()
                    .RegisterDomainShipsServices()
                    .RegisterDomainTeamsServices()
                    .RegisterDomainGraphicsServices();

                builder.Configure<LoggerConfiguration>((scope, config) =>
                {
                    config.Destructure.AsScalar<VhId>();
                });

                builder.RegisterType<UniqueNumberProvider>().As<IUniqueNumberProvider>().InstancePerLifetimeScope();

                builder.RegisterJsonConverter<Fix64Converter>();
                builder.RegisterJsonConverter<FixPolarConverter>();
                builder.RegisterJsonConverter<FixVector2Converter>();
                builder.RegisterJsonConverter<NativeDynamicArrayCastJsonConverter>();
                builder.RegisterJsonConverter<KeyConverter>();

                builder.RegisterPolymorphicJsonType<Fix64, object>(nameof(Fix64));
            });
        }
    }
}

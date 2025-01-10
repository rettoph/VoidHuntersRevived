using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Common.Extensions;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Pieces.Engines;
using VoidHuntersRevived.Domain.Pieces.ResourceTypes;
using VoidHuntersRevived.Domain.Pieces.Serialization.Components;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Pieces.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDomainPiecesServices(this ContainerBuilder builder) => builder.EnsureRegisteredOnce(nameof(RegisterDomainPiecesServices), builder =>
                                                                                                               {
                                                                                                                   builder.RegisterType<BlueprintService>().AsImplementedInterfaces().SingleInstance();
                                                                                                                   builder.RegisterType<TreeService>().AsImplementedInterfaces().InstancePerLifetimeScope();
                                                                                                                   builder.RegisterType<NodeService>().AsImplementedInterfaces().InstancePerLifetimeScope();
                                                                                                                   builder.RegisterType<SocketService>().AsImplementedInterfaces().InstancePerLifetimeScope();

                                                                                                                   builder.RegisterJsonConverter<BlueprintConverter>();
                                                                                                                   builder.RegisterJsonConverter<BlueprintPieceConverter>();
                                                                                                                   builder.RegisterJsonConverter<ShapeJsonConverter>();
                                                                                                                   builder.RegisterJsonConverter<SocketsJsonConverter>();
                                                                                                                   builder.RegisterJsonConverter<SocketJsonConverter>();
                                                                                                                   builder.RegisterJsonConverter<PlugJsonConverter>();
                                                                                                                   builder.RegisterJsonConverter<ThrustableJsonConverter>();

                                                                                                                   builder.RegisterPolymorphicJsonType<Plug, IEntityComponent>(nameof(Plug));
                                                                                                                   builder.RegisterPolymorphicJsonType<Sockets, IEntityComponent>(nameof(Sockets));
                                                                                                                   builder.RegisterPolymorphicJsonType<Thrustable, IEntityComponent>(nameof(Thrustable));
                                                                                                                   builder.RegisterPolymorphicJsonType<Primitive<VertexVisible>, IEntityComponent>("Primitive.Visible");
                                                                                                                   builder.RegisterPolymorphicJsonType<PrimitiveSequenceGroup<VertexVisible>, IEntityComponent>("PrimitiveSequenceGroup.Visible");

                                                                                                                   builder.RegisterEngine<CouplingEngine>();
                                                                                                                   builder.RegisterEngine<NodeFixtureEngine>();
                                                                                                                   builder.RegisterEngine<SocketIdsEngine>();
                                                                                                                   builder.RegisterEngine<ThrustableEngine>();
                                                                                                                   builder.RegisterEngine<TractorableEngine>();
                                                                                                                   builder.RegisterEngine<TreeEngine>();

                                                                                                                   builder.RegisterComponentSerializer<CouplingComponentSerializer>();
                                                                                                                   builder.RegisterComponentSerializer<NodeComponentSerializer>();
                                                                                                                   builder.RegisterComponentSerializer<PlugComponentSerializer>();
                                                                                                                   builder.RegisterComponentSerializer<SocketIdsComponentSerializer>();
                                                                                                                   builder.RegisterComponentSerializer<TreeComponentSerializer>();

                                                                                                                   builder.RegisterResourceType<BlueprintResourceType>();

                                                                                                                   builder.Configure<LoggerConfiguration>((scope, config) =>
                                                                                                                   {
                                                                                                                       config.Destructure.AsScalar(typeof(Id<Blueprint>));
                                                                                                                   });
                                                                                                               });
    }
}
using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Resources.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Game.Common.Extensions;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Pieces.Systems;
using VoidHuntersRevived.Domain.Pieces.ResourceTypes;
using VoidHuntersRevived.Domain.Pieces.Serialization.Components;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Pieces.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterDomainPiecesServices(this IGuppyScopeBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainPiecesServices), builder =>
            {
                builder.RegisterResourceType<BlueprintResourceType>();

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

                builder.RegisterComponentSerializer<CouplingComponentSerializer>();
                builder.RegisterComponentSerializer<NodeComponentSerializer>();
                builder.RegisterComponentSerializer<PlugComponentSerializer>();
                builder.RegisterComponentSerializer<SocketIdsComponentSerializer>();
                builder.RegisterComponentSerializer<TreeComponentSerializer>();

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterType<BlueprintService>().AsImplementedInterfaces().SingleInstance();
                    builder.RegisterType<TreeService>().AsImplementedInterfaces().InstancePerLifetimeScope();
                    builder.RegisterType<NodeService>().AsImplementedInterfaces().InstancePerLifetimeScope();
                    builder.RegisterType<SocketService>().AsImplementedInterfaces().InstancePerLifetimeScope();

                    builder.RegisterEngine<CouplingSystem>();
                    builder.RegisterEngine<NodeFixtureSystem>();
                    builder.RegisterEngine<SocketIdsSystem>();
                    builder.RegisterEngine<ThrustableSystem>();
                    builder.RegisterEngine<TractorableSystem>();
                    builder.RegisterEngine<TreeSystem>();
                });
            });
        }
    }
}
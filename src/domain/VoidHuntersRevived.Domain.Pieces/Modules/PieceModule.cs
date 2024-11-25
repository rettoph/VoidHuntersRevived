using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Core.Resources.Serialization.Json;
using Serilog;
using Svelto.ECS;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Pieces.Engines;
using VoidHuntersRevived.Domain.Pieces.ResourceTypes;
using VoidHuntersRevived.Domain.Pieces.Serialization.Components;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Pieces.Modules
{
    public sealed class PieceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<BlueprintService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TreeService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<NodeService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SocketService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<BlueprintConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<BlueprintPieceConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<RigidJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<ShapeJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<SocketsJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<LocationJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<PlugJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<ThrustableJsonConverter>().As<JsonConverter>().SingleInstance();

            builder.RegisterEngine<CouplingEngine>();
            builder.RegisterEngine<NodeEngine>();
            builder.RegisterEngine<RigidEngine>();
            builder.RegisterEngine<SocketIdsEngine>();
            builder.RegisterEngine<ThrustableEngine>();
            builder.RegisterEngine<TractorableEngine>();
            builder.RegisterEngine<TreeEngine>();

            builder.RegisterInstance<PolymorphicJsonType>(new PolymorphicJsonType<Primitive<VertexVisible>, IEntityComponent>("Primitive.Visible")).SingleInstance();
            builder.RegisterInstance<PolymorphicJsonType>(new PolymorphicJsonType<PrimitiveSequenceGroup<VertexVisible>, IEntityComponent>("PrimitiveSequenceGroup.Visible")).SingleInstance();

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
        }
    }
}

using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Serialization.Json;
using Guppy.Engine.Common.Loaders;
using Serilog;
using Svelto.ECS;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Loaders
{
    [AutoLoad]
    public sealed class PieceLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
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

            builder.RegisterInstance<PolymorphicJsonType>(new PolymorphicJsonType<PrimitiveComponent<VertexVisible>, IEntityComponent>("Primitive.Visible")).SingleInstance();
            builder.RegisterInstance<PolymorphicJsonType>(new PolymorphicJsonType<PrimitiveSequenceGroup<VertexVisible>, IEntityComponent>("PrimitiveSequenceGroup.Visible")).SingleInstance();

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar(typeof(Id<Blueprint>));
            });
        }
    }
}

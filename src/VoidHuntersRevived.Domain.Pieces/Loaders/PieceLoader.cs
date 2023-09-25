using Autofac;
using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.Loaders;
using Guppy.Network;
using Guppy.Resources.Serialization.Json.Converters;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Loaders
{
    [AutoLoad]
    public sealed class PieceLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<DictionaryPolymorphicConverter<IPieceComponent>>().As<JsonConverter>().SingleInstance();
            services.RegisterType<RigidJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<VisibleJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<ShapeJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<SocketsJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<LocationJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<PlugJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<ThrustableJsonConverter>().As<JsonConverter>().SingleInstance();

            services.RegisterType<PieceService>().AsImplementedInterfaces().SingleInstance();
            services.RegisterType<BlueprintService>().AsImplementedInterfaces().SingleInstance();
            
            services.RegisterType<TreeService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<NodeService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<SocketService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<VisibleRenderingService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar(typeof(Id<IBlueprint>));
            });
        }
    }
}

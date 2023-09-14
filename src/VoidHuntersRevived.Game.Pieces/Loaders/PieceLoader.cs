using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Network;
using Guppy.Resources.Serialization.Json.Converters;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Domain.Serialization.Json;
using VoidHuntersRevived.Game.Pieces.Serialization.Json;
using VoidHuntersRevived.Game.Pieces.Services;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class PieceLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<PolymorphicConverter<IPieceComponent>>().As<JsonConverter>().SingleInstance();
            services.RegisterType<NativeDynamicArrayCastJsonConverter<Polygon>>().As<JsonConverter>().SingleInstance();
            services.RegisterType<NativeDynamicArrayCastJsonConverter<Shape>>().As<JsonConverter>().SingleInstance();
            services.RegisterType<RigidJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<VisibleJsonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<ShapeJsonConverter>().As<JsonConverter>().SingleInstance();

            services.RegisterType<PieceService>().AsImplementedInterfaces().SingleInstance();
            
            services.RegisterType<TreeService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<NodeService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<SocketService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<VisibleRenderingService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}

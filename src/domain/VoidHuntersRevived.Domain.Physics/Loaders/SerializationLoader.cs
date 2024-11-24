using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Physics.Serialization.Components;
using VoidHuntersRevived.Domain.Physics.Serialization.Json;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    internal sealed class SerializationLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<PolygonConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<BodyTemplateConverter>().As<JsonConverter>().SingleInstance();

            services.RegisterComponentSerializer<AwakeComponentSerializer>();
            services.RegisterComponentSerializer<CollisionComponentSerializer>();
            services.RegisterComponentSerializer<EnabledComponentSerializer>();
            services.RegisterComponentSerializer<LocationComponentSerializer>();
            services.RegisterComponentSerializer<PhysicsBubbleComponentSerializer>();
        }
    }
}

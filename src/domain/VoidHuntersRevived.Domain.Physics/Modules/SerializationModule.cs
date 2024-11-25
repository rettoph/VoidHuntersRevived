using Autofac;
using Guppy.Core.Common.Attributes;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Physics.Serialization.Components;
using VoidHuntersRevived.Domain.Physics.Serialization.Json;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;

namespace VoidHuntersRevived.Domain.Physics.Modules
{
    [AutoLoad]
    internal sealed class SerializationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<PolygonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<BodyTemplateConverter>().As<JsonConverter>().SingleInstance();

            builder.RegisterComponentSerializer<AwakeComponentSerializer>();
            builder.RegisterComponentSerializer<CollisionComponentSerializer>();
            builder.RegisterComponentSerializer<EnabledComponentSerializer>();
            builder.RegisterComponentSerializer<LocationComponentSerializer>();
            builder.RegisterComponentSerializer<PhysicsBubbleComponentSerializer>();
        }
    }
}

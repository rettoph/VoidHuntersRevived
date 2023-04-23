using Guppy.Loaders;
using Guppy.Resources.Serialization.Json.Converters;
using Guppy.Resources.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Domain.Entities.Serialization.Json.Converters;
using System.Text.Json.Serialization;
using Guppy.Attributes;
using tainicom.Aether.Physics2D.Collision.Shapes;
using Guppy.Attributes;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal sealed class SerializationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<JsonConverter, ShipPartResourceConverter>();
            services.AddSingleton<PolymorphicJsonType>(new PolymorphicJsonType<ShipPartResource>(PolymorphicJsonTypes.ShipPartResource));

            services.AddSingleton<JsonConverter, PolymorphicEnumerableConverter<Shape>>();
            services.AddSingleton<JsonConverter, PolygonShapeConverter>();
            services.AddSingleton<PolymorphicJsonType>(new PolymorphicJsonType<PolygonShape>(nameof(ShapeType.Polygon)));

            services.AddSingleton<JsonConverter, PolymorphicEnumerableConverter<IShipPartComponentConfiguration>>();
            services.AddSingleton<PolymorphicJsonType>(new PolymorphicJsonType<DrawConfiguration>(PolymorphicJsonTypes.DrawableConfiguration));
            services.AddSingleton<PolymorphicJsonType>(new PolymorphicJsonType<RigidConfiguration>(PolymorphicJsonTypes.RigidConfiguration));
        }
    }
}

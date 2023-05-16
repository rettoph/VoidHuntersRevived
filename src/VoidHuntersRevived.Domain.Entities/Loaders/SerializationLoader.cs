using Guppy.Loaders;
using Guppy.Resources.Serialization.Json.Converters;
using Guppy.Resources.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts;
using System.Text.Json.Serialization;
using Guppy.Attributes;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Domain.Entities.Serialization.Json.Converters;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal sealed class SerializationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<JsonConverter, PolymorphicEnumerableConverter<Shape>>();
            services.AddSingleton<JsonConverter, PolygonShapeConverter>();
            services.AddSingleton<PolymorphicJsonType>(new PolymorphicJsonType<PolygonShape>(nameof(ShapeType.Polygon)));
        }
    }
}

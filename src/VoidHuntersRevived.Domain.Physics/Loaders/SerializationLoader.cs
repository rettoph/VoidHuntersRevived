using Guppy.Loaders;
using Guppy.Resources.Serialization.Json.Converters;
using Guppy.Resources.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Guppy.Attributes;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    internal sealed class SerializationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<JsonConverter, PolygonConverter>();
        }
    }
}

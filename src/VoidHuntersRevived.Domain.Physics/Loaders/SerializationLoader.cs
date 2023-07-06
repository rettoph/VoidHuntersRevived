using Guppy.Loaders;
using System.Text.Json.Serialization;
using Guppy.Attributes;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;
using Autofac;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    internal sealed class SerializationLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<PolygonConverter>().As<JsonConverter>().SingleInstance();
        }
    }
}

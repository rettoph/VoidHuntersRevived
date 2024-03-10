using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;

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

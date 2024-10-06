using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Serialization.Json;
using VoidHuntersRevived.Domain.Graphics.Services;

namespace VoidHuntersRevived.Domain.Graphics.Loaders
{
    [AutoLoad]
    public sealed class GraphicsLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<PrimitiveService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterGeneric(typeof(PrimitiveService<>)).As(typeof(IPrimitiveService<>)).SingleInstance();

            builder.RegisterType<PrimitiveContextConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<PrimitiveConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<PrimitiveSequenceGroupConverter>().As<JsonConverter>().SingleInstance();
        }
    }
}

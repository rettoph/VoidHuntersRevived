using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Engine.Common.Loaders;
using Serilog;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Serialization.Json;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Services;

namespace VoidHuntersRevived.Domain.Teams.Loaders
{
    [AutoLoad]
    public sealed class TeamLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<TeamJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<ColorSchemeJsonConverter>().As<JsonConverter>().SingleInstance();
            builder.RegisterType<zIndexJsonConverter>().As<JsonConverter>().SingleInstance();

            builder.RegisterType<TeamService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar(typeof(Id<Team>));
            });
        }
    }
}

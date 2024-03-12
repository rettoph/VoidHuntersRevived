using Autofac;
using Guppy.Attributes;
using Guppy.Extensions.Autofac;
using Guppy.Loaders;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Teams;
using VoidHuntersRevived.Domain.Teams.Common.Services;
using VoidHuntersRevived.Domain.Teams.Services;

namespace VoidHuntersRevived.Domain.Teams.Loaders
{
    [AutoLoad]
    public sealed class TeamLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<TeamService>().As<ITeamService>().InstancePerLifetimeScope();

            builder.RegisterType<TeamDescriptorGroupService>().As<ITeamDescriptorGroupService>().InstancePerLifetimeScope();

            builder.Configure<LoggerConfiguration>((scope, config) =>
            {
                config.Destructure.AsScalar(typeof(Id<ITeam>));
            });
        }
    }
}

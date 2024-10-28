using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Teams.Services;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Registration.Teams.Extensions
{
    public static class ServiceCollectionMockerExtensions
    {
        public static void RegisterTeamsServices(this ServiceCollectionMocker services)
        {
            services.RegisterFactory<TeamService>(provider => new(
                provider.Get<IEntityTemplateService>(),
                provider.Get<IEntityQueryService>(),
                provider.Get<IPrivateEntitySpawnService>()));
        }
    }
}

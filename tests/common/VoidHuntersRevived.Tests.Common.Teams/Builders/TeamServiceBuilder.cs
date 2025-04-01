using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Teams.Services;
using VoidHuntersRevived.Tests.Common.Entities.Builders;

namespace VoidHuntersRevived.Tests.Common.Teams.Builders
{
    public class TeamServiceBuilder : Builder<TeamService>
    {
        public required EntityTemplateServiceBuilder EntityTemplateServiceBuilder { get; init; }
        public required EntitySpawnServiceBuilder EntitySpawnServiceBuilder { get; init; }

        protected override TeamService Build()
        {
            return new TeamService(
                entityTemplateService: this.EntityTemplateServiceBuilder.Object,
                privateEntitySpawnService: this.EntitySpawnServiceBuilder.Object);
        }
    }
}

using VoidHuntersRevived.Domain.Teams.Systems;
using VoidHuntersRevived.Tests.Common.Entities.Mockers;
using VoidHuntersRevived.Tests.Common.Teams.Builders;
using VoidHuntersRevived.Tests.Common.Teams.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Teams.Mockers
{
    public class TeamPredictiveStrategyMocker : EntityPredictiveStrategyMocker, ITeamStrategyMocker
    {
        public TeamServiceBuilder TeamServiceBuilder { get; }

        public TeamPredictiveStrategyMocker()
        {
            this.TeamServiceBuilder = new TeamServiceBuilder()
            {
                EntityTemplateServiceBuilder = this.EntityTemplateServiceBuilder,
                EntitySpawnServiceBuilder = this.EntitySpawnServiceBuilder
            };

            this.SystemFactories.AddRange([
                x => new TeamServiceInitializationSystem(
                    teamService: this.TeamServiceBuilder.Object),
                x => new ColorSchemeSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object)
            ]);
        }
    }
}

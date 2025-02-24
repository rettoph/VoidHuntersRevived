using VoidHuntersRevived.Tests.Common.Entities.Mockers;
using VoidHuntersRevived.Tests.Domain.Entities.Systems;
using VoidHuntersRevived.Tests.Entities.Stubs;

namespace VoidHuntersRevived.Tests.Domain.Entities.Builders
{
    public class TestEntityPredictiveStrategyBuilder : EntityPredictiveStrategyMocker
    {
        public TestEntityPredictiveStrategyBuilder()
        {
            this.SystemFactories.AddRange([
                x => new TestInputSystem(
                    entitySpawnService: this.EntitySpawnServiceBuilder.Object)
            ]);

            this.EntityTemplateFragmentServiceMocker.EntityTemplateFragments.AddRange([
                TestEntityComponent.TestEntityTemplateFragment
            ]);
        }
    }
}

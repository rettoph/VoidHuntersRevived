using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Common.Simulations;
using VoidHuntersRevived.Tests.Common.Simulations.Strategies;

namespace VoidHuntersRevived.Tests.Domain.Pieces
{
    public class TractorBeamEmmiter_AttachDetach_Tests : BaseSimulationTests<TractorBeamEmmiter_AttachDetach_Tests>
    {
        public TractorBeamEmmiter_AttachDetach_Tests() : base([
            typeof(ClientLockstepStrategyBuilder),
            typeof(PredictiveStrategyBuilder)
        ])
        {
        }

        [Fact]
        public void Test1()
        {

        }

        protected override IEnumerable<IEngine> GetEngines(IStrategyBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IEntityTemplate> GetEntityTemplates(IStrategyBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
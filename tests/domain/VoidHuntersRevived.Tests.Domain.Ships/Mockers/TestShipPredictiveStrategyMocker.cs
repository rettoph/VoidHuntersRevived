using VoidHuntersRevived.Tests.Common.Ships.Mockers;
using VoidHuntersRevived.Tests.Domain.Ships.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Ships.Mockers
{
    public class TestShipPredictiveStrategyMocker : ShipPredictiveStrategyMocker
    {
        public TestShipPredictiveStrategyMocker()
        {
            this.EntityTemplateFragmentServiceMocker.RegisterTestShipEntityTemplateFragments();
        }
    }
}

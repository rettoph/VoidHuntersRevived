using VoidHuntersRevived.Tests.Common.Ships.Mockers;
using VoidHuntersRevived.Tests.Domain.Ships.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Ships.Mockers
{
    public class TestShipLockstepStrategyMocker : ShipLockstepStrategyMocker
    {
        public TestShipLockstepStrategyMocker()
        {
            this.EntityTemplateFragmentServiceMocker.RegisterTestShipEntityTemplateFragments();
        }
    }
}

using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using VoidHuntersRevived.Tests.Common.Physics.Interfaces;
using VoidHuntersRevived.Tests.Common.Ships.Builders;

namespace VoidHuntersRevived.Tests.Common.Ships.Interfaces
{
    public interface IShipStrategyMocker : IPhysicsStrategyMocker
    {
        BlueprintServiceBuilder BlueprintServiceBuilder { get; }
        TreeServiceBuilder TreeServiceBuilder { get; }
        NodeServiceBuilder NodeServiceBuilder { get; }
        NodeSocketServiceBuilder NodeSocketServiceBuilder { get; }
        TractorBeamEmitterServiceBuilder TractorBeamEmitterServiceBuilder { get; }
        TacticalServiceBuilder TacticalServiceBuilder { get; }
        Mocker<IResourceService> ResourceServiceMocker { get; }
    }
}

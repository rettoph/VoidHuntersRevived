using Autofac;
using Guppy.Core.Network.Common.Enums;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Common.Extensions
{
    public static class AutoMockBuilderExtensions
    {
        public static AutoMockBuilder RegisterSimulation(this AutoMockBuilder builder, VhId id, PeerType peerType, Dictionary<StrategyTypeEnum, IEngine[]> strategies)
        {
            return builder.Register(x => x.Register<ISimulation>(ctx => SimulationFactory.Build(id, peerType, strategies)).AsSelf().AsImplementedInterfaces());
        }
    }
}

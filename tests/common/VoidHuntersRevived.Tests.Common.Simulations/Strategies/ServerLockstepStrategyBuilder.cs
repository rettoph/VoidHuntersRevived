using Guppy.Core.Network.Common.Enums;
using Guppy.Tests.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Tests.Common.Simulations.Strategies
{
    public sealed class ServerLockstepStrategyBuilder : BaseStrategyBuilder
    {
        public ServerLockstepStrategyBuilder() : base(PeerType.Server, StrategyTypeEnum.Lockstep)
        {
        }

        protected override IStrategy build(ISimulation simulation)
        {
            return new LockstepStrategy_Server(
                this.Bus.GetInstance(),
                this.SettingService.GetInstance(),
                simulation.ToLazy(),
                this.EngineServiceBuilder.GetLazy<IEngineService>(),
                this.Logger.GetLazy());
        }
    }
}

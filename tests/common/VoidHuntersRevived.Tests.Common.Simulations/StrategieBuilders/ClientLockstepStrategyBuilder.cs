using Guppy.Core.Network.Common.Enums;
using Guppy.Tests.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Tests.Common.Simulations.Strategies
{
    public sealed class ClientLockstepStrategyBuilder : BaseStrategyBuilder
    {
        public ClientLockstepStrategyBuilder() : base(PeerType.Client, StrategyTypeEnum.Lockstep)
        {
        }

        protected override IStrategy build()
        {
            return new LockstepStrategy_Client(
                this.NetScope.GetInstance(),
                this.TickBuffer.GetInstance(),
                this.SettingService.GetInstance(),
                this.EngineServiceBuilder.GetLazy<IEngineService>(),
                this.Logger.GetInstance().ToLazy());
        }
    }
}

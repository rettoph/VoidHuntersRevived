using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class LockstepStrategyMocker : BaseStrategyMocker<LockstepStrategy_Client>
    {
        public Mocker<INetScope<IStrategy>> NetScopeMocker { get; set; }
        public Mocker<TickBuffer> TickBufferMocker { get; set; }
        public Mocker<ISettingService> SettingServiceMocker { get; set; }
        public LockstepStepEventServiceMocker LockstepStepEventServiceMocker { get; set; }

        public LockstepStrategyMocker() : base()
        {
            this.NetScopeMocker = new Mocker<INetScope<IStrategy>>();
            this.TickBufferMocker = new Mocker<TickBuffer>();
            this.SettingServiceMocker = new Mocker<ISettingService>();
            this.LockstepStepEventServiceMocker = new LockstepStepEventServiceMocker();
        }

        protected override LockstepStrategy_Client Build()
        {
            return new LockstepStrategy_Client(
                netScope: this.NetScopeMocker.GetInstance(),
                ticks: this.TickBufferMocker.GetInstance(),
                settings: this.SettingServiceMocker.GetInstance(),
                scope: this.GuppyScopeMocker.GetInstance(),
                eventService: this.LockstepStepEventServiceMocker.LockstepEventService,
                logger: this.LoggerMocker.GetInstance());
        }
    }
}

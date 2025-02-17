using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Domain.Simulations.Systems;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class DefaultLockstepStrategyMocker : BaseStrategyMocker<LockstepStrategy>
    {
        public static readonly int DefaultStepsPerTick = 3;
        public static readonly Fix64 DefaultStepInterval = (Fix64)20 / (Fix64)1000;

        public Mocker<ISettingService> SettingServiceMocker { get; set; }
        public ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; set; }
        public DefaultLockstepStepEventServiceMocker DefaultLockstepStepEventServiceMocker { get; set; }
        public DefaultLockstepTickServiceMocker DefaultLockstepTickServiceMocker { get; set; }
        public DefaultLockstepStepServiceMocker DefaultLockstepStepServiceMocker { get; set; }

        public DefaultLockstepStrategyMocker() : this(DefaultStepsPerTick, DefaultStepInterval)
        {

        }
        public DefaultLockstepStrategyMocker(int stepsPerTick, Fix64 stepInterval) : base()
        {
            this.SettingServiceMocker = new Mocker<ISettingService>();
            this.ChannelMessageBusProxyMocker = new ChannelMessageBusProxyMocker();
            this.DefaultLockstepStepEventServiceMocker = new DefaultLockstepStepEventServiceMocker()
            {
                LoggerMocker = this.LoggerMocker,
                MessageBusMocker = this.ChannelMessageBusProxyMocker.MessageBusMocker
            };
            this.DefaultLockstepTickServiceMocker = new DefaultLockstepTickServiceMocker()
            {
                SettingServiceMocker = this.SettingServiceMocker,
                DefaultLockstepStepEventServiceMocker = this.DefaultLockstepStepEventServiceMocker
            };
            this.DefaultLockstepStepServiceMocker = new DefaultLockstepStepServiceMocker()
            {
                SettingServiceMocker = this.SettingServiceMocker,
                DefaultLockstepTickServiceMocker = this.DefaultLockstepTickServiceMocker,
                MessageBusMocker = this.ChannelMessageBusProxyMocker.MessageBusMocker
            };

            this.ChannelMessageBusProxyMocker.ProxyPublish<StepSequenceGroupEnum, Step>();
            this.ChannelMessageBusProxyMocker.ProxyPublish<TickSequenceGroupEnum, Tick>();

            this.SettingServiceMocker.Setup(Settings.StepsPerTick, stepsPerTick);
            this.SettingServiceMocker.Setup(Settings.StepInterval, stepInterval);

            this.SystemFactories.AddRange([
                x => new StepServiceUpdateSystem(this.DefaultLockstepStepServiceMocker.GetInstance())
            ]);
        }

        protected override LockstepStrategy Build()
        {
            return new LockstepStrategy(
                settings: this.SettingServiceMocker.GetInstance(),
                scope: this.GuppyScopeMocker.GetInstance(),
                eventService: this.DefaultLockstepStepEventServiceMocker.DefaultLockstepStepEventService,
                logger: this.LoggerMocker.GetInstance());
        }
    }
}

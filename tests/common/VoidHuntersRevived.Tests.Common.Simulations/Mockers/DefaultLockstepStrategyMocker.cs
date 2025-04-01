using Guppy.Core.Messaging.Common.Services;
using Guppy.Core.Messaging.Systems.Scoped;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Domain.Simulations.Systems;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class DefaultLockstepStrategyMocker : StrategyMocker<LockstepStrategy>
    {
        public static readonly int DefaultStepsPerTick = 3;
        public static readonly Fix64 DefaultStepInterval = (Fix64)20 / (Fix64)1000;

        public Mocker<ISettingService> SettingServiceMocker { get; set; }
        public ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; }
        public DefaultLockstepStepEventServiceBuilder DefaultLockstepStepEventServiceBuilder { get; }
        public DefaultLockstepTickServiceBuilder DefaultLockstepTickServiceBuilder { get; }
        public DefaultLockstepStepServiceBuilder DefaultLockstepStepServiceBuilder { get; }

        public DefaultLockstepStrategyMocker() : this(DefaultStepsPerTick, DefaultStepInterval)
        {

        }
        public DefaultLockstepStrategyMocker(int stepsPerTick, Fix64 stepInterval) : base()
        {
            this.SettingServiceMocker = new Mocker<ISettingService>();
            this.ChannelMessageBusProxyMocker = new ChannelMessageBusProxyMocker()
            {
                MessageBusServiceMocker = new Mocker<IMessageBusService>()
            };
            this.DefaultLockstepStepEventServiceBuilder = new DefaultLockstepStepEventServiceBuilder()
            {
                LoggerMocker = this.LoggerMocker,
                ChannelMessageBusProxyMocker = this.ChannelMessageBusProxyMocker
            };
            this.DefaultLockstepTickServiceBuilder = new DefaultLockstepTickServiceBuilder()
            {
                SettingServiceMocker = this.SettingServiceMocker,
                DefaultLockstepStepEventServiceMocker = this.DefaultLockstepStepEventServiceBuilder
            };
            this.DefaultLockstepStepServiceBuilder = new DefaultLockstepStepServiceBuilder()
            {
                SettingServiceMocker = this.SettingServiceMocker,
                DefaultLockstepTickServiceBuilder = this.DefaultLockstepTickServiceBuilder,
                ChannelMessageBusProxyMocker = this.ChannelMessageBusProxyMocker
            };

            this.SettingServiceMocker.SetupReturn(Settings.StepsPerTick, stepsPerTick);
            this.SettingServiceMocker.SetupReturn(Settings.StepInterval, stepInterval);

            this.SystemFactories.AddRange([
                x => new StepServiceUpdateSystem(this.DefaultLockstepStepServiceBuilder.Object),
                x => new StepEventServiceFlushSystem(this.DefaultLockstepStepEventServiceBuilder.Object),
                x => new AutoSubscribeScopedSystemsToBrokerServiceSystem(
                    messageBus: this.ChannelMessageBusProxyMocker.Object,
                    scopedSystemService: this.ScopedSystemServiceMocker.Object)
            ]);
        }

        protected override LockstepStrategy Build()
        {
            return new LockstepStrategy(
                settings: this.SettingServiceMocker.Object,
                scope: this.GuppyScopeMocker.Object,
                eventService: this.DefaultLockstepStepEventServiceBuilder.Object,
                logger: this.LoggerMocker.Object);
        }
    }
}

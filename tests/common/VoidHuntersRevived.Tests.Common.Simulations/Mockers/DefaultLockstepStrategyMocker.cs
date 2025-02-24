using Guppy.Core.Messaging.Common.Services;
using Guppy.Core.Messaging.Systems.Scoped;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Builders;
using Guppy.Tests.Common.Extensions;
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
        public ChannelMessageBusBuilder ChannelMessageBusBuilder { get; }
        public DefaultLockstepStepEventServiceBuilder DefaultLockstepStepEventServiceMocker { get; }
        public DefaultLockstepTickServiceBuilder DefaultLockstepTickServiceMocker { get; }
        public DefaultLockstepStepServiceBuilder DefaultLockstepStepServiceMocker { get; }

        public DefaultLockstepStrategyMocker() : this(DefaultStepsPerTick, DefaultStepInterval)
        {

        }
        public DefaultLockstepStrategyMocker(int stepsPerTick, Fix64 stepInterval) : base()
        {
            this.SettingServiceMocker = new Mocker<ISettingService>();
            this.ChannelMessageBusBuilder = new ChannelMessageBusBuilder()
            {
                MessageBusServiceMocker = new Mocker<IMessageBusService>()
            };
            this.DefaultLockstepStepEventServiceMocker = new DefaultLockstepStepEventServiceBuilder()
            {
                LoggerMocker = this.LoggerMocker,
                ChannelMessageBusBuilder = this.ChannelMessageBusBuilder
            };
            this.DefaultLockstepTickServiceMocker = new DefaultLockstepTickServiceBuilder()
            {
                SettingServiceMocker = this.SettingServiceMocker,
                DefaultLockstepStepEventServiceMocker = this.DefaultLockstepStepEventServiceMocker
            };
            this.DefaultLockstepStepServiceMocker = new DefaultLockstepStepServiceBuilder()
            {
                SettingServiceMocker = this.SettingServiceMocker,
                DefaultLockstepTickServiceBuilder = this.DefaultLockstepTickServiceMocker,
                ChannelMessageBusBuilder = this.ChannelMessageBusBuilder
            };

            this.SettingServiceMocker.SetupReturn(Settings.StepsPerTick, stepsPerTick);
            this.SettingServiceMocker.SetupReturn(Settings.StepInterval, stepInterval);

            this.SystemFactories.AddRange([
                x => new StepServiceUpdateSystem(this.DefaultLockstepStepServiceMocker.Object),
                x => new StepEventServiceFlushSystem(this.DefaultLockstepStepEventServiceMocker.Object),
                x => new AutoSubscribeScopedSystemsToBrokerServiceSystem(
                    messageBus: this.ChannelMessageBusBuilder.Object,
                    scopedSystemService: this.ScopedSystemServiceMocker.Object)
            ]);
        }

        protected override LockstepStrategy Build()
        {
            return new LockstepStrategy(
                settings: this.SettingServiceMocker.Object,
                scope: this.GuppyScopeMocker.Object,
                eventService: this.DefaultLockstepStepEventServiceMocker.Object,
                logger: this.LoggerMocker.Object);
        }
    }
}

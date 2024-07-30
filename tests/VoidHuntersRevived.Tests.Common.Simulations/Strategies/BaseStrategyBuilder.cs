using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Serilog;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Tests.Common.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Strategies
{
    public abstract class BaseStrategyBuilder : BaseInstanceBuilder<ISimulation, IStrategy>, IStrategyBuilder
    {
        public PeerType PeerType { get; }
        public StrategyTypeEnum Type { get; }
        public EngineServiceBuilder EngineServiceBuilder { get; }
        public Mocker<ILogger> Logger { get; }
        public Mocker<ISettingService> SettingService { get; }
        public Mocker<INetScope<IStrategy>> NetScope { get; }
        public Mocker<TickBuffer> TickBuffer { get; }
        public Mocker<IBus> Bus { get; }

        protected BaseStrategyBuilder(PeerType peerType, StrategyTypeEnum type)
        {
            this.PeerType = peerType;
            this.Type = type;
            this.EngineServiceBuilder = new EngineServiceBuilder();
            this.Logger = new Mocker<ILogger>();
            this.SettingService = new Mocker<ISettingService>();
            this.NetScope = new Mocker<INetScope<IStrategy>>();
            this.TickBuffer = new Mocker<TickBuffer>();
            this.Bus = new Mocker<IBus>();
        }
    }
}

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
    public abstract class BaseStrategyBuilder(PeerType peerType, StrategyTypeEnum type) : BaseInstanceBuilder<IStrategy>, IStrategyBuilder
    {
        public PeerType PeerType { get; } = peerType;
        public StrategyTypeEnum Type { get; } = type;
        public EngineServiceBuilder EngineServiceBuilder { get; } = new EngineServiceBuilder();
        public Mocker<ILogger> Logger { get; } = new Mocker<ILogger>();
        public Mocker<ISettingService> SettingService { get; } = new Mocker<ISettingService>();
        public Mocker<INetScope<IStrategy>> NetScope { get; } = new Mocker<INetScope<IStrategy>>();
        public Mocker<TickBuffer> TickBuffer { get; } = new Mocker<TickBuffer>();
        public Mocker<IBus> Bus { get; } = new Mocker<IBus>();
    }
}

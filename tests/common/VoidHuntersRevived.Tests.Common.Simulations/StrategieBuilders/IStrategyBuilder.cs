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
    public interface IStrategyBuilder : IInstanceBuilder<IStrategy>
    {
        PeerType PeerType { get; }
        StrategyTypeEnum Type { get; }

        EngineServiceBuilder EngineServiceBuilder { get; }
        Mocker<ISettingService> SettingService { get; }
        Mocker<INetScope<IStrategy>> NetScope { get; }
        Mocker<TickBuffer> TickBuffer { get; }
        Mocker<IBus> Bus { get; }
        Mocker<ILogger> Logger { get; }
    }
}

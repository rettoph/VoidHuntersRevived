using Guppy.Core.Network.Common.Enums;
using Guppy.Tests.Common;
using Moq;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;
using VoidHuntersRevived.Tests.Common.Simulations.Strategies;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class StrategiesFactoryBuilder : BaseInstanceBuilder<IStrategiesFactory>
    {
        private Mocker<IStrategiesFactory> _mocker;
        private IStrategyBuilder[] _builders;

        public PeerType PeerType;

        public Mocker<IStrategiesFactory> Mocker => _mocker;

        public StrategiesFactoryBuilder(PeerType peerType)
        {
            _mocker = new Mocker<IStrategiesFactory>();
            _builders = [
                new PredictiveStrategyBuilder(),
                new ClientLockstepStrategyBuilder(),
                new ServerLockstepStrategyBuilder()
            ];

            this.PeerType = peerType;

            this.Mocker.Setup<IEnumerable<IStrategy>, ISimulation, StrategyTypeEnum[]>(
                expression: factory => factory.BuildStrategies(It.IsAny<ISimulation>(), It.IsAny<StrategyTypeEnum[]>()),
                result: this.BuildStrategiesMock);
        }

        private IEnumerable<IStrategy> BuildStrategiesMock(ISimulation simulation, StrategyTypeEnum[] strategies)
        {
            return this.GetStrategyBuilders(strategies)
                .Select(x => x.BuildInstance(simulation))
                .ToList();
        }

        public IEnumerable<IStrategyBuilder> GetStrategyBuilders(StrategyTypeEnum[]? strategies = null)
        {
            return _builders.Where(x => x.PeerType == this.PeerType && (strategies is null || strategies.Contains(x.Type)));
        }

        public IStrategyBuilder GetStrategyBuilder(StrategyTypeEnum strategy)
        {
            return this.GetStrategyBuilders().First(x => x.Type == strategy);
        }

        public StrategiesFactoryBuilder Configure(Action<IStrategyBuilder> configuration)
        {
            foreach (IStrategyBuilder builder in this.GetStrategyBuilders())
            {
                configuration(builder);
            }

            return this;
        }

        public StrategiesFactoryBuilder Configure(StrategyTypeEnum strategy, Action<IStrategyBuilder> configuration)
        {
            configuration(this.GetStrategyBuilder(strategy));

            return this;
        }

        public StrategiesFactoryBuilder Configure(StrategyTypeEnum[] strategies, Action<IStrategyBuilder> configuration)
        {
            foreach (IStrategyBuilder builder in this.GetStrategyBuilders(strategies))
            {
                configuration(builder);
            }

            return this;
        }

        protected override IStrategiesFactory build()
        {
            return _mocker.GetInstance();
        }
    }
}
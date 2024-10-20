using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations
{
    public sealed class Simulation : ISimulation
    {
        private readonly Dictionary<StrategyTypeEnum, IStrategy> _strategyTypes;
        private readonly List<IStrategy> _strategies;

        public VhId Id { get; }

        public IStrategy this[StrategyTypeEnum type] => _strategyTypes[type];

        public IReadOnlyCollection<IStrategy> Strategies { get; }

        public Simulation(VhId id, IEnumerable<IStrategy> strategies)
        {
            _strategies = strategies.ToList();
            _strategyTypes = _strategies.ToDictionary(x => x.Type, x => x);

            this.Id = id;
            this.Strategies = new ReadOnlyCollection<IStrategy>(_strategies);

            foreach (IStrategy strategy in _strategies)
            {
                strategy.Initialize(this);
            }
        }

        public void Dispose()
        {
            foreach (IStrategy strategy in _strategies)
            {
                strategy.Dispose();
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = _strategies.Count - 1; i >= 0; i--)
            {
                _strategies[i].Draw(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _strategies.Count; i++)
            {
                _strategies[i].Update(gameTime);
            }
        }

        public IStrategy? First(params StrategyTypeEnum[] strategies)
        {
            foreach (StrategyTypeEnum strategyType in strategies)
            {
                if (_strategyTypes.TryGetValue(strategyType, out IStrategy? strategy))
                {
                    return strategy;
                }
            }

            return null;
        }

        public void Input(VhId sourceId, IInputData data)
        {
            foreach (IStrategy strategy in _strategies)
            {
                strategy.Input(sourceId, data);
            }
        }
    }
}

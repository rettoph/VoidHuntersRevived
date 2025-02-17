using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations
{
    public sealed class Simulation : ISimulation
    {
        private readonly List<IStrategy> _strategies;

        public VhId Id { get; }

        public IEnumerable<IStrategy> this[StrategyTypeEnum type] => this._strategies.Where(x => x.Type == type);

        public IReadOnlyCollection<IStrategy> Strategies { get; }

        public Simulation(VhId id, Func<ISimulation, IEnumerable<IStrategy>> strategiesBuilder)
        {
            this._strategies = strategiesBuilder(this).ToList();

            this.Id = id;
            this.Strategies = new ReadOnlyCollection<IStrategy>(this._strategies);
        }

        public void Initialize()
        {
            // Ensure all internal strategies are initialized
            foreach (IStrategy strategy in this._strategies)
            {
                strategy.Initialize();
            }
        }

        public void Dispose()
        {
            foreach (IStrategy strategy in this._strategies)
            {
                strategy.Dispose();
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = this._strategies.Count - 1; i >= 0; i--)
            {
                this._strategies[i].Draw(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < this._strategies.Count; i++)
            {
                this._strategies[i].Update(gameTime);
            }
        }

        public IStrategy? First(params StrategyTypeEnum[] strategies)
        {
            foreach (StrategyTypeEnum strategyType in strategies)
            {
                IStrategy? result = this._strategies.FirstOrDefault(x => x.Type == strategyType);
                if (result is not null)
                {
                    return result;
                }
            }

            return null;
        }

        public void Input(VhId sourceId, IStepInput input)
        {
            EnqueuedStepInput enqueuedStepInput = new(sourceId, input);
            foreach (IStrategy strategy in this._strategies)
            {
                strategy.Events.Input(enqueuedStepInput);
            }
        }
    }
}
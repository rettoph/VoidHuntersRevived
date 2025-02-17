using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public interface ISimulation : IDisposable
    {
        public VhId Id { get; }
        IReadOnlyCollection<IStrategy> Strategies { get; }

        IEnumerable<IStrategy> this[StrategyTypeEnum type] { get; }

        void Draw(GameTime gameTime);

        void Update(GameTime gameTime);

        /// <summary>
        /// Publish input event
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="input"></param>
        void Input(VhId sourceId, IStepInput input);

        /// <summary>
        /// Iterate through the given <paramref name="strategies"/> and return the first
        /// matching <see cref="IStrategy"/> instance, if any
        /// </summary>
        /// <param name="strategies"></param>
        /// <returns></returns>
        IStrategy? First(params StrategyTypeEnum[] strategies);
    }
}
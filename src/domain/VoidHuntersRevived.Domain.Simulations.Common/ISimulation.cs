using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public interface ISimulation : IDisposable
    {
        public VhId Id { get; }
        IReadOnlyCollection<IStrategy> Strategies { get; }

        IStrategy this[StrategyTypeEnum type] { get; }

        void Draw(GameTime gameTime);

        void Update(GameTime gameTime);

        /// <summary>
        /// Publish input event
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="data"></param>
        void Input(VhId sourceId, IInputData data);

        /// <summary>
        /// Iterate through the given <paramref name="strategies"/> and return the first
        /// matching <see cref="IStrategy"/> instance, if any
        /// </summary>
        /// <param name="strategies"></param>
        /// <returns></returns>
        IStrategy? First(params StrategyTypeEnum[] strategies);
    }
}

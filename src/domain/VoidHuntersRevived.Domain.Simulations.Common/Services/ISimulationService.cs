using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface ISimulationService
    {
        ReadOnlyCollection<ISimulation> Instances { get; }

        ISimulation Create(VhId id, params StrategyTypeEnum[] strategies);

        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }
}
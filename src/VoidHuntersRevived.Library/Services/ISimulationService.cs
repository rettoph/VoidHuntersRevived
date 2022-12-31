using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Services
{
    public interface ISimulationService
    {
        SimulationType Flags { get; }

        SimulationEntityMapper EntityMapper { get; }

        IEnumerable<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Initialize(SimulationType flags);
        void Update(GameTime gameTime);
    }
}

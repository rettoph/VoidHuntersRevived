using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Maps;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Services
{
    public interface ISimulationService
    {
        SimulationType Flags { get; }

        UserIdSimulatedIdMap UserIdMap { get; }

        IEnumerable<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Initialize(SimulationType flags);
        void Update(GameTime gameTime);
    }
}

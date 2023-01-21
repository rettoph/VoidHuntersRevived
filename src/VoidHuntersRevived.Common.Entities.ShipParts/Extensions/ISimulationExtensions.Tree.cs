using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateChain(this ISimulation simulation, ParallelKey key, string headConfiguration)
        {
            var head = simulation.CreateShipPart(key.Create(ParallelTypes.Chain, 0), headConfiguration);

            return simulation.CreateEntity(key).MakeChain(simulation.Aether, head);
        }
    }
}

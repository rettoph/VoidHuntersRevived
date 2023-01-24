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
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateChain(this ISimulation simulation, ParallelKey key, string headConfiguration, Vector2 position = default, float rotation = 0)
        {
            var head = simulation.CreateShipPart(key.Create(ParallelTypes.ShipPart, 0), headConfiguration);

            return simulation.CreateEntity(key).MakeChain(simulation.Aether, head.Id, position, rotation);
        }
        public static Entity CreateChain(this ISimulation simulation, ParallelKey key, int headId, Vector2 position = default, float rotation = 0)
        {
            return simulation.CreateEntity(key).MakeChain(simulation.Aether, headId, position, rotation);
        }
    }
}

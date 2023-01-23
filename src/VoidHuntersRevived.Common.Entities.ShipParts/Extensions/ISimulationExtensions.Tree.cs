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
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateChain(this ISimulation simulation, ParallelKey key, Entity head, Vector2 position = default, float rotation = 0)
        {
            simulation.PublishEvent(new CreateChain(
                key: key,
                head: head,
                position: position, 
                rotation: rotation));

            return simulation.GetEntity(key);
        }
    }
}

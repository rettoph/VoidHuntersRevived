using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface IChainService
    {
        void MakeChain(Entity entity, Node head, Vector2 position, float rotation, ISimulationEvent @event);
        Entity CreateChain(string headResource, Vector2 position, float rotation, ISimulationEvent @event);
        Entity CreateChain(Node head, Vector2 position, float rotation, ISimulationEvent @event);
    }
}

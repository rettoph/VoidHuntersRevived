using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class ChainService : IChainService
    {
        private readonly IShipPartService _shipParts;
        private readonly ITreeService _trees;

        public ChainService(IShipPartService shipParts, ITreeService trees)
        {
            _shipParts = shipParts;
            _trees = trees;
        }

        public void MakeChain(Entity entity, Node head, Vector2 position, float rotation, ISimulationEvent @event)
        {
            Body body = @event.Simulation.Aether.CreateBody(bodyType: BodyType.Dynamic);
            body.SetTransformIgnoreContacts(ref position, rotation);
            body.OnCollision += HandleChainCollision;

            _trees.MakeTree(entity, body, head);

            entity.Attach(new Tractorable(entity.Id));
        }

        public Entity CreateChain(string headResource, Vector2 position, float rotation, ISimulationEvent @event)
        {
            Entity shipPart = _shipParts.CreateShipPart(headResource, @event);
            Node head = shipPart.Get<Node>();

            return this.CreateChain(head, position, rotation, @event);
        }

        public Entity CreateChain(Node head, Vector2 position, float rotation, ISimulationEvent @event)
        {
            Entity chain = @event.Simulation.CreateEntity(@event.NewKey());
            this.MakeChain(chain, head, position, rotation, @event);

            return chain;
        }

        private static bool HandleChainCollision(Fixture sender, Fixture other, Contact contact)
        {
            return false;
        }
    }
}

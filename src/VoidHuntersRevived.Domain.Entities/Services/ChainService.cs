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

        public void MakeChain(Entity entity, Node head, Vector2 position, float rotation, ISimulation simulation)
        {
            Body body = simulation.Aether.CreateBody(bodyType: BodyType.Dynamic);
            body.SetTransformIgnoreContacts(ref position, rotation);
            body.OnCollision += HandleChainCollision;

            _trees.MakeTree(entity, body, head);

            entity.Attach(Tractorable.Instance);
        }

        public Entity CreateChain(ParallelKey key, string headResource, Vector2 position, float rotation, ISimulation simulation)
        {
            Entity shipPart = _shipParts.CreateShipPart(key.Create(ParallelTypes.ShipPart, 0), simulation, headResource);
            Node head = shipPart.Get<Node>();

            return this.CreateChain(key, head, position, rotation, simulation);
        }

        public Entity CreateChain(ParallelKey key, Node head, Vector2 position, float rotation, ISimulation simulation)
        {
            Entity chain = simulation.CreateEntity(key);
            this.MakeChain(chain, head, position, rotation, simulation);

            return chain;
        }

        private static bool HandleChainCollision(Fixture sender, Fixture other, Contact contact)
        {
            return false;
        }
    }
}

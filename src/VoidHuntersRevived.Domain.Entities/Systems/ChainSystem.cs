using Guppy.Common;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class ChainSystem : BasicSystem,
        ISubscriber<IEvent<CreateChain>>
    {
        public void Process(in IEvent<CreateChain> message)
        {
            var body = message.Simulation.Aether.CreateBody(bodyType: BodyType.Dynamic);
            body.OnCollision += HandleChainCollision;

            message.Simulation.PublishEvent(new CreateTree(
                key: message.Data.Key,
                body: body,
                head: message.Data.Head,
                position: message.Data.Position,
                rotation: message.Data.Rotation));

            var chain = message.Simulation.GetEntity(message.Data.Key);
            chain.Attach(Tractorable.Instance);
        }

        private static bool HandleChainCollision(Fixture sender, Fixture other, Contact contact)
        {
            return false;
        }
    }
}

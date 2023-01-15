using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class RigidSystem : EntitySystem,
        ISubscriber<IEvent<CleanLink>>
    {
        private ComponentMapper<Rigid> _rigids;
        private ComponentMapper<ShipPartLeaf> _leaves;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Linked> _linked;

        public RigidSystem() : base(Aspect.All(typeof(Rigid), typeof(ShipPartLeaf)))
        {
            _rigids = default!;
            _leaves = default!;
            _bodies = default!;
            _linked = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _rigids = mapperService.GetMapper<Rigid>();
            _leaves = mapperService.GetMapper<ShipPartLeaf>();
            _bodies = mapperService.GetMapper<Body>();
            _linked = mapperService.GetMapper<Linked>();
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            var leaf = _leaves.Get(entityId);
            var rigid = _rigids.Get(entityId);
            var body = _bodies.Get(leaf.Tree.Entity.Id);
            var link = _linked.Get(entityId);

            var transformation = link is null ? Matrix.Identity : link.LocalTransformation;

            foreach(var shape in rigid.Configuration.Shapes)
            {
                var fixture = new Fixture(shape.Clone(ref transformation));
                body.Add(fixture);

                var coords = string.Join(',', ((PolygonShape)fixture.Shape).Vertices.Select(x => $"({x.X},{x.Y})"));
                Console.WriteLine(coords);
            }
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            if (!this.subscription.IsInterested(entityId))
            {
                return;
            }
        }

        public void Process(in IEvent<CleanLink> message)
        { // TODO: This should probably be in a AetherLeafSystem
            if(!this.subscription.IsInterested(message.Data.Link.Parent.Linkable.Entity.Id))
            {
                return;
            }

            // The entity is currently attached to another tree
            if (this.subscription.IsInterested(message.Data.Link.Joint.Linkable.Entity.Id))
            { // Detach it from its old tree
                var leaf = _leaves.Get(message.Data.Link.Joint.Linkable.Entity.Id)!;
                leaf.Tree.Remove(leaf);
            }

            var tree = _leaves.Get(message.Data.Link.Parent.Linkable.Entity.Id)!.Tree;
            tree.Add(message.Data.Link.Joint.Linkable.Entity);
        }
    }
}

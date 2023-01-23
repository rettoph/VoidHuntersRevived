using Guppy.Common;
using Guppy.Common.Collections;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class RigidNodeSystem : EntitySystem
    {
        private ComponentMapper<Rigid> _rigids;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Body> _bodies;
        private Queue<Fixture> _buffer;

        public RigidNodeSystem() : base(Aspect.All(typeof(Rigid), typeof(Node)))
        {
            _buffer = new Queue<Fixture>();

            _rigids = default!;
            _nodes = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _rigids = mapperService.GetMapper<Rigid>();
            _nodes = mapperService.GetMapper<Node>();
            _bodies = mapperService.GetMapper<Body>();
        }

        private void AddRigid(int entityId)
        {
            var node = _nodes.Get(entityId);
            var rigid = _rigids.Get(entityId);
            var body = _bodies.Get(node.Tree);

            var transformation = node.LocalTransformation;

            foreach (var shape in rigid.Configuration.Shapes)
            {
                var fixture = new Fixture(shape.Clone(ref transformation));
                fixture.Tag = entityId;

                body.Add(fixture);
            }

            _bodies.Put(entityId, body);
        }

        private void RemoveRigid(int entityId)
        {
            var body = _bodies.Get(entityId);

            foreach (var fixture in body.FixtureList)
            {
                if (fixture.Tag is int fixtureEntityId && entityId == fixtureEntityId)
                {
                    _buffer.Enqueue(fixture);
                }
            }

            while (_buffer.TryDequeue(out var fixture))
            {
                body.Remove(fixture);
            }

            _bodies.Delete(entityId);
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            this.AddRigid(entityId);
        }

        protected override void OnEntityChanged(int entityId, BitVector32 oldBits)
        {
            base.OnEntityChanged(entityId, oldBits);

            bool wasInterested = this.subscription.IsInterested(oldBits);
            bool isInterested = this.subscription.IsInterested(entityId);

            if (wasInterested == isInterested)
            {
                return;
            }

            if(wasInterested)
            {
                this.RemoveRigid(entityId);
            }

            if (isInterested)
            {
                this.AddRigid(entityId);
            }
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            if (!this.subscription.IsInterested(entityId))
            {
                return;
            }

            this.RemoveRigid(entityId);
        }
    }
}

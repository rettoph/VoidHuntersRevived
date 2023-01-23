using Guppy.Common;
using Guppy.Common.Collections;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
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
    internal sealed class RigidNodeSystem : EntitySystem,
        ISubscriber<IEvent<CleanJointed>>
    {
        private ComponentMapper<Rigid> _rigids;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Body> _bodies;
        private Queue<Fixture> _buffer;
        private Dictionary<int, Fixture[]> _fixtures;

        public RigidNodeSystem() : base(Aspect.All(typeof(Rigid), typeof(Node)))
        {
            _buffer = new Queue<Fixture>();
            _fixtures = new Dictionary<int, Fixture[]>();

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
            var fixtures = new Fixture[rigid.Configuration.Shapes.Length];

            for(var i=0; i<rigid.Configuration.Shapes.Length; i++)
            {
                fixtures[i] = new Fixture(rigid.Configuration.Shapes[i].Clone(ref transformation));
                fixtures[i].Tag = entityId;

                body.Add(fixtures[i]);
            }

            _fixtures.Add(entityId, fixtures);
        }

        private void RemoveRigid(int entityId)
        {
            foreach(var fixture in _fixtures[entityId])
            {
                fixture.Body.Remove(fixture);
            }

            _fixtures.Remove(entityId);
        }

        private void UpdateRigid(int entityId)
        {
            bool interested = this.subscription.IsInterested(entityId);
            bool added = _fixtures.ContainsKey(entityId);

            if (interested && !added)
            {
                this.AddRigid(entityId);
                return;
            }

            if (!interested && added)
            {
                this.RemoveRigid(entityId);
                return;
            }
        }

        public void Process(in IEvent<CleanJointed> message)
        {
            this.UpdateRigid(message.Data.Jointed.Joint.Entity.Id);
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            this.UpdateRigid(entityId);
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            this.UpdateRigid(entityId);
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
    }
}

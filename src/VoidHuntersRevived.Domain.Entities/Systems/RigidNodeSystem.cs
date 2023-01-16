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

        public RigidNodeSystem() : base(Aspect.All(typeof(Rigid), typeof(Node)))
        {
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

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            var node = _nodes.Get(entityId);
            var rigid = _rigids.Get(entityId);
            var body = _bodies.Get(node.Tree.Entity.Id);

            var transformation = node.LocalTransformation;

            foreach(var shape in rigid.Configuration.Shapes)
            {
                var fixture = new Fixture(shape.Clone(ref transformation));
                body.Add(fixture);
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
    }
}

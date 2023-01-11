using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class HullSystem : EntitySystem
    {
        private ComponentMapper<Hull> _hulls;
        private ComponentMapper<AetherLeaf> _leaves;
        private ComponentMapper<Body> _bodies;

        public HullSystem() : base(Aspect.All(typeof(Hull), typeof(AetherLeaf)))
        {
            _hulls = default!;
            _leaves = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _hulls = mapperService.GetMapper<Hull>();
            _leaves = mapperService.GetMapper<AetherLeaf>();
            _bodies = mapperService.GetMapper<Body>();
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            var leaf = _leaves.Get(entityId);
            var hull = _hulls.Get(entityId);
            var body = _bodies.Get(leaf.Tree.Entity.Id);

            foreach(var shape in hull.Shapes)
            {
                var fixture = new Fixture(shape.Polygon.Clone());
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

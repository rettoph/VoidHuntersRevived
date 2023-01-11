using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class RigidShipPartSystem : EntitySystem
    {
        private ComponentMapper<Rigid> _rigids;
        private ComponentMapper<AetherLeaf> _leaves;
        private ComponentMapper<Body> _bodies;

        public RigidShipPartSystem() : base(Aspect.All(typeof(Rigid), typeof(AetherLeaf)))
        {
            _rigids = default!;
            _leaves = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _rigids = mapperService.GetMapper<Rigid>();
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
            var rigid = _rigids.Get(entityId);
            var body = _bodies.Get(leaf.Tree.Entity.Id);

            foreach(var shape in rigid.Shapes)
            {
                var fixture = new Fixture(shape.Clone());
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

using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.ShipParts.Systems
{
    internal sealed class RigidSystem : EntitySystem
    {
        private static readonly AspectBuilder RigidAspect = Aspect.All(new[]
        {
            typeof(Rigid),
            typeof(ShipPart),
            typeof(Parallelable)
        });

        private ComponentMapper<Rigid> _rigids = null!;
        private ComponentMapper<Body> _bodies = null!;
        private ComponentMapper<ShipPart> _shipParts = null!;
        private ComponentMapper<Fixture[]> _fixtures = null!;
        private ComponentMapper<Parallelable> _parallelables = null!;

        public RigidSystem() : base(RigidAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _rigids = mapperService.GetMapper<Rigid>();
            _shipParts = mapperService.GetMapper<ShipPart>();
            _bodies = mapperService.GetMapper<Body>();
            _fixtures = mapperService.GetMapper<Fixture[]>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            ShipPart shipPart = _shipParts.Get(entityId);
            Rigid rigid = _rigids.Get(entityId);
            Parallelable parallelable = _parallelables.Get(entityId);
            if (!_bodies.TryGet(entityId, out Body? body))
            {
                return;
            }

            FixedMatrix transformation = FixedMatrix.Identity;
            Fixture[] fixtures = new Fixture[rigid.Shapes.Length];

            for (var i = 0; i < rigid.Shapes.Length; i++)
            {
                fixtures[i] = new Fixture(rigid.Shapes[i].Clone(ref transformation));
                fixtures[i].Tag = parallelable.Key;

                body.Add(fixtures[i]);
            }

            _fixtures.Put(entityId, fixtures);
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            if (!this.subscription.IsInterested(entityId))
            {
                return;
            }

            Fixture[] fixtures = _fixtures.Get(entityId);
            Body body = _bodies.Get(entityId);

            foreach(Fixture fixture in fixtures)
            {
                body.Remove(fixture);
                fixture.Tag = null;
            }
        }
    }
}

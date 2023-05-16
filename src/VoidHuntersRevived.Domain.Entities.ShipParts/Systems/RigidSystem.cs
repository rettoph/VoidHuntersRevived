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

namespace VoidHuntersRevived.Domain.Entities.ShipParts.Systems
{
    internal sealed class RigidSystem : EntitySystem
    {
        private static readonly AspectBuilder RigidAspect = Aspect.All(new[]
        {
            typeof(Rigid),
            typeof(ShipPart)
        });

        private ComponentMapper<Rigid> _rigids = null!;
        private ComponentMapper<Body> _bodies = null!;
        private ComponentMapper<ShipPart> _shipParts = null!;
        private ComponentMapper<Fixture[]> _fixtures = null!;

        public RigidSystem() : base(RigidAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _rigids = mapperService.GetMapper<Rigid>();
            _shipParts = mapperService.GetMapper<ShipPart>();
            _bodies = mapperService.GetMapper<Body>();
            _fixtures = mapperService.GetMapper<Fixture[]>();
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
            if (!_bodies.TryGet(shipPart.Head.EntityId ?? -1, out Body? body))
            {
                return;
            }

            Matrix transformation = Matrix.Identity;
            Fixture[] fixtures = new Fixture[rigid.Shapes.Length];

            for (var i = 0; i < rigid.Shapes.Length; i++)
            {
                fixtures[i] = new Fixture(rigid.Shapes[i].Clone(ref transformation));
                fixtures[i].Tag = entityId;

                body.Add(fixtures[i]);
            }

            _fixtures.Put(entityId, fixtures);
        }
    }
}

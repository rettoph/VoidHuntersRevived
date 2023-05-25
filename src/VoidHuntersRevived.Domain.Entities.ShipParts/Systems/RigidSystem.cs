using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics;

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
        private ComponentMapper<ShipPart> _shipParts = null!;
        private ComponentMapper<IBody> _bodies = null!;
        private ComponentMapper<IFixture[]> _fixtures = null!;
        private ComponentMapper<Parallelable> _parallelables = null!;

        public RigidSystem() : base(RigidAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _rigids = mapperService.GetMapper<Rigid>();
            _shipParts = mapperService.GetMapper<ShipPart>();
            _bodies = mapperService.GetMapper<IBody>();
            _fixtures = mapperService.GetMapper<IFixture[]>();
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
            if (!_bodies.TryGet(entityId, out IBody? body))
            {
                return;
            }

            FixMatrix transformation = FixMatrix.Identity;
            IFixture[] fixtures = new IFixture[rigid.Polygons.Length];

            for (var i = 0; i < rigid.Polygons.Length; i++)
            {
                fixtures[i] = body.Create(rigid.Polygons[i], parallelable.Key);
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

            IFixture[] fixtures = _fixtures.Get(entityId);
            IBody body = _bodies.Get(entityId);

            foreach(IFixture fixture in fixtures)
            {
                body.Destroy(fixture);
            }
        }
    }
}

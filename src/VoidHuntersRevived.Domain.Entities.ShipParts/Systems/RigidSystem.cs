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
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Entities.ShipParts.Systems
{
    internal sealed class RigidSystem : ParallelEntitySystem
    {
        private static readonly AspectBuilder RigidAspect = Aspect.All(new[]
        {
            typeof(Rigid),
            typeof(ShipPart),
        });

        private IParallelComponentMapper<Rigid> _rigids = null!;
        private IParallelComponentMapper<ShipPart> _shipParts = null!;
        private IParallelComponentMapper<IBody> _bodies = null!;
        private IParallelComponentMapper<IFixture[]> _fixtures = null!;

        public RigidSystem() : base(RigidAspect)
        {
        }

        public override void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            base.Initialize(components, entities);

            _rigids = components.GetMapper<Rigid>();
            _shipParts = components.GetMapper<ShipPart>();
            _bodies = components.GetMapper<IBody>();
            _fixtures = components.GetMapper<IFixture[]>();
        }

        protected override void OnEntityAdded(ParallelKey entityKey, ISimulation simulation)
        {
            base.OnEntityAdded(entityKey, simulation);

            if (!this.Entities[simulation.Type].IsInterested(entityKey))
            {
                return;
            }

            ShipPart shipPart = _shipParts.Get(entityKey, simulation);
            Rigid rigid = _rigids.Get(entityKey, simulation);

            if (!_bodies.TryGet(entityKey, simulation, out IBody ? body))
            {
                return;
            }

            FixMatrix transformation = FixMatrix.Identity;
            IFixture[] fixtures = new IFixture[rigid.Polygons.Length];

            for (var i = 0; i < rigid.Polygons.Length; i++)
            {
                fixtures[i] = body.Create(rigid.Polygons[i], entityKey);
            }

            _fixtures.Put(entityKey, simulation, fixtures);
        }

        protected override void OnEntityRemoved(ParallelKey entityKey, ISimulation simulation)
        {
            base.OnEntityRemoved(entityKey, simulation);

            if (!this.Entities[simulation.Type].IsInterested(entityKey))
            {
                return;
            }

            IFixture[] fixtures = _fixtures.Get(entityKey, simulation);
            IBody body = _bodies.Get(entityKey, simulation);

            foreach (IFixture fixture in fixtures)
            {
                body.Destroy(fixture);
            }
        }
    }
}

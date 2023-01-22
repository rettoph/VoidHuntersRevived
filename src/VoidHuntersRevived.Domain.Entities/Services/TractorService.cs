using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class TractorService : ITractorService, ISystem
    {
        public ISimulation TryGetTractorableSimulation;
        public const float TryGetTractorableRadius = 5f;

        private readonly ISimulationService _simulations;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Parallelable> _parallelables;
        private Vector2 _target;
        private float _distance;
        private int? _tractorableId;
        private ParallelKey _tractorableKey;

        public TractorService(ISimulationService simulations)
        {
            _simulations = simulations;
            _tractorables = default!;
            _nodes = default!;
            _parallelables = default!;

            this.TryGetTractorableSimulation = default!;
        }

        public void Initialize(World world)
        {
            _tractorables = world.ComponentMapper.GetMapper<Tractorable>();
            _nodes = world.ComponentMapper.GetMapper<Node>();
            _parallelables = world.ComponentMapper.GetMapper<Parallelable>();

            this.TryGetTractorableSimulation = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTractorable(Vector2 target, out ParallelKey tractorable)
        {
            _distance = float.MaxValue;
            _tractorableId = null;

            AABB aabb = new AABB(target, TryGetTractorableRadius, TryGetTractorableRadius);
            this.TryGetTractorableSimulation.Aether.QueryAABB(this.TractorableCallback, ref aabb);
            
            if(_tractorableId is null)
            {
                tractorable = default;
                return false;
            }

            tractorable = _tractorableKey;
            return true;
        }

        public bool CanTractor(Vector2 target, ParallelKey tractorable)
        {
            return true;
        }

        private bool TractorableCallback(Fixture fixture)
        {
            if(fixture.Tag is not int entityId)
            { // If the fixture is not bound to an entity...
                return true;
            }
            
            if(!_nodes.TryGet(entityId, out var node))
            { // If the entity is not a node...
                return true;
            }

            if(_tractorableId == node.Tree.Id)
            { // This is already the target
                return true;
            }

            if(!_tractorables.Has(node.Tree.Id))
            { // If the node is not attached to a tractorable...
                return true;
            }

            var position = Vector2.Transform(Vector2.Zero, node.WorldTransformation);
            var distance = Vector2.Distance(_target, position);

            if(distance >= _distance)
            { // The new distance is further away than the previously closest found target
                return true;
            }

            // Update the target tractorable
            _distance = distance;
            _tractorableId = node.Tree.Id;
            _tractorableKey = _parallelables.Get(node.Tree.Id).Key;

            return true;
        }
    }
}

using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System.Diagnostics.CodeAnalysis;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
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
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Parallelable> _parallelables;
        private ComponentMapper<Tree> _trees;
        private Vector2 _target;
        private float _distance;
        private int? _tractorableId;
        private ParallelKey _tractorableKey;

        public TractorService(ISimulationService simulations)
        {
            _simulations = simulations;
            _tractorables = default!;
            _bodies = default!;
            _nodes = default!;
            _jointables = default!;
            _parallelables = default!;
            _trees = default!;

            this.TryGetTractorableSimulation = default!;
        }

        public void Initialize(World world)
        {
            _tractorables = world.ComponentMapper.GetMapper<Tractorable>();
            _bodies = world.ComponentMapper.GetMapper<Body>();
            _nodes = world.ComponentMapper.GetMapper<Node>();
            _jointables = world.ComponentMapper.GetMapper<Jointable>();
            _parallelables = world.ComponentMapper.GetMapper<Parallelable>();
            _trees = world.ComponentMapper.GetMapper<Tree>();

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

        public bool TryGetPotentialParentJoint(
            Vector2 target,
            Tractoring tractoring,
            [MaybeNullWhen(false)] out Vector2 position, 
            [MaybeNullWhen(false)] out Jointable.Joint parent)
        {
            parent = null;
            position = default;
            float distance = 5f;
            var tree = _trees.Get(tractoring.EntityId);
            var body = _bodies.Get(tractoring.EntityId);

            foreach (Entity nodeEntity in tree.Nodes)
            {
                var node = _nodes.Get(nodeEntity);
                var jointable = _jointables.Get(nodeEntity);

                if(node is null || jointable is null)
                {
                    continue;
                }

                foreach(var joint in jointable.Joints)
                {
                    if(joint.Jointed)
                    {
                        continue;
                    }

                    var jointPosition = body.Position + joint.LocalPosition;
                    var jointDistance = Vector2.Distance(target, jointPosition);

                    if(jointDistance < distance)
                    {
                        distance = jointDistance;
                        parent = joint;
                        position = jointPosition;
                    }
                }
            }

            return parent is not null;
        }

        public bool TransformTractorable(
            Vector2 target,
            Tractoring tractoring,
            [MaybeNullWhen(false)] out Jointing potential)
        {
            var tractorableBody = _bodies.Get(tractoring.TractorableId);
            var tractoringTree = _trees.Get(tractoring.EntityId);

            if (!this.TryGetPotentialParentJoint(target, tractoring, out Vector2 position, out var tractoringJoint))
            {
                return this.DefaultTransformBody(tractorableBody, target, out potential);
            }

            var tractoringBody = _bodies.Get(tractoring.EntityId);
            var tractorableTree = _trees.Get(tractoring.TractorableId);
            var tractorableJointable = _jointables.Get(tractorableTree.Head);
            var tractorableJoint = tractorableJointable.Joints.FirstOrDefault(x => !x.Jointed);

            if (tractorableJoint is null)
            {
                return this.DefaultTransformBody(tractorableBody, target, out potential);
            }

            potential = new Jointing(tractorableJoint, tractoringJoint);
            var transformation = potential.LocalTransformation * tractoringBody.GetTransformation();

            target = Vector2.Transform(Vector2.Zero, transformation);
            var rotation = transformation.Radians();
            tractorableBody.SetTransformIgnoreContacts(target, -rotation);

            return true;
        }

        private bool DefaultTransformBody(Body body, Vector2 target, out Jointing? potential)
        {
            potential = null;
            target = Vector2.Transform(target, body.GetLocalCenterTransformation().Invert());
            body.SetTransformIgnoreContacts(target, body.Rotation);

            return false;
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

            var distance = Vector2.Distance(_target, node.WorldPosition);

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

using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System.Diagnostics.CodeAnalysis;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed partial class TractorService : ITractorService, ISystem
    {
        public const float GetPotentialParentJointDistance = 1f;

        private readonly ISimulationService _simulations;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Parallelable> _parallelables;
        private ComponentMapper<Tree> _trees;
        private ComponentMapper<ShipPartConfiguration> _shipParts;
        private ISimulation _interactive;

        public TractorService(ISimulationService simulations)
        {
            _simulations = simulations;
            _tractorables = default!;
            _bodies = default!;
            _nodes = default!;
            _jointables = default!;
            _parallelables = default!;
            _trees = default!;
            _shipParts = default!;

            _interactive = default!;
        }

        public void Initialize(World world)
        {
            _tractorables = world.ComponentMapper.GetMapper<Tractorable>();
            _bodies = world.ComponentMapper.GetMapper<Body>();
            _nodes = world.ComponentMapper.GetMapper<Node>();
            _jointables = world.ComponentMapper.GetMapper<Jointable>();
            _parallelables = world.ComponentMapper.GetMapper<Parallelable>();
            _trees = world.ComponentMapper.GetMapper<Tree>();
            _shipParts = world.ComponentMapper.GetMapper<ShipPartConfiguration>();

            _interactive = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryGetTractorable(Pilotable pilotable, out ParallelKey shipPartKey, out ParallelKey nodeKey)
        {
            return QueryTractorable.Invoke(pilotable, this, out shipPartKey, out nodeKey);
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
            float distance = GetPotentialParentJointDistance;
            var tree = _trees.Get(tractoring.EntityId);
            var body = _bodies.Get(tractoring.EntityId);

            foreach (int nodeId in tree.Nodes)
            {
                var node = _nodes.Get(nodeId);
                var jointable = _jointables.Get(nodeId);

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
            var tractorableJointable = _jointables.Get(tractorableTree.HeadId.Value);
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
            target -= body.WorldCenter - body.Position;
            body.SetTransformIgnoreContacts(target, body.Rotation);

            return false;
        }
    }
}

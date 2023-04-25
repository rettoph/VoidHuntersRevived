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
        public const float GetPotentialChildJointDistance = 1f;

        private readonly ISimulationService _simulations;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Parallelable> _parallelables;
        private ComponentMapper<Tree> _trees;
        private ComponentMapper<ShipPartResource> _shipParts;
        private ISimulation _interactive;

        public TractorService(ISimulationService simulations)
        {
            _simulations = simulations;
            _tractorables = default!;
            _bodies = default!;
            _nodes = default!;
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
            _parallelables = world.ComponentMapper.GetMapper<Parallelable>();
            _trees = world.ComponentMapper.GetMapper<Tree>();
            _shipParts = world.ComponentMapper.GetMapper<ShipPartResource>();

            _interactive = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep);
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public bool TryGetTractorable(Pilotable pilotable, out ParallelKey targetTreeKey, out ParallelKey targetNodeKey)
        {
            return QueryTractorable.Invoke(pilotable, this, out targetTreeKey, out targetNodeKey);
        }

        public bool CanTractor(Vector2 target, ParallelKey tractorable)
        {
            return true;
        }

        public bool TryGetPotentialLink(
            Vector2 target,
            int tractoringId,
            [MaybeNullWhen(false)] out Vector2 position,
            [MaybeNullWhen(false)] out Joint parentJoint)
        {
            parentJoint = null;
            position = default;
            float distance = GetPotentialChildJointDistance;
            var tree = _trees.Get(tractoringId);
            var body = _bodies.Get(tractoringId);

            foreach (Node node in tree.Nodes)
            {
                foreach (Joint joint in node.Joints)
                {
                    if (joint.Link is not null)
                    {
                        continue;
                    }

                    var jointPosition = body.Position + joint.LocalPosition;
                    var jointDistance = Vector2.Distance(target, jointPosition);

                    if (jointDistance < distance)
                    {
                        distance = jointDistance;
                        parentJoint = joint;
                        position = jointPosition;
                    }
                }
            }

            return parentJoint is not null;
        }

        public bool TransformTractorable(
            Vector2 target,
            Tractoring tractoring,
            [MaybeNullWhen(false)] out Link potential)
        {
            return this.TransformTractorable(target, tractoring.EntityId, tractoring.TargetTreeId, out potential);
        }

        public bool TransformTractorable(
            Vector2 target, 
            int tractoringId, 
            int tractorableId, 
            [MaybeNullWhen(false)] out Link potential)
        {
            Body tractorableBody = _bodies.Get(tractorableId);
            Tree tractoringTree = _trees.Get(tractoringId);

            if (!this.TryGetPotentialLink(target, tractoringId, out Vector2 position, out Joint? parentJoint))
            {
                return this.DefaultTransformBody(tractorableBody, target, out potential);
            }

            Body tractoringBody = _bodies.Get(tractoringId);
            Tree tractorableTree = _trees.Get(tractorableId);

            if (tractorableTree?.Head is null)
            {
                return this.DefaultTransformBody(tractorableBody, target, out potential);
            }

            Joint? childJoint = tractorableTree.Head.Joints.FirstOrDefault(x => x.Link is null);

            if (childJoint is null)
            {
                return this.DefaultTransformBody(tractorableBody, target, out potential);
            }

            potential = new Link(childJoint, parentJoint);
            var transformation = potential.LocalTransformation * tractoringBody.GetTransformation();

            target = Vector2.Transform(Vector2.Zero, transformation);
            var rotation = transformation.Radians();
            tractorableBody.SetTransformIgnoreContacts(target, -rotation);

            return true;
        }

        private bool DefaultTransformBody(Body? body, Vector2 target, out Link? potential)
        {
            potential = null;

            if(body is not null)
            {
                target -= body.WorldCenter - body.Position;
                body.SetTransformIgnoreContacts(target, body.Rotation);
            }

            return false;
        }
    }
}

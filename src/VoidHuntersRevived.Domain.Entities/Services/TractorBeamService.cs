using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.Tractoring;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;
using VoidHuntersRevived.Common.Entities.Tractoring.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class TractorBeamService : ITractorBeamService, ISystem
    {
        private ComponentMapper<TractorBeamEmitter> _emitters = null!;
        private ComponentMapper<Pilotable> _pilotables = null!;
        private ComponentMapper<Tractorable> _tractorables = null!;
        private ComponentMapper<Node> _nodes = null!;
        private ComponentMapper<Body> _bodies = null!;
        private ComponentMapper<Tree> _trees = null!;

        public void Initialize(World world)
        {
            _emitters = world.ComponentMapper.GetMapper<TractorBeamEmitter>();
            _pilotables = world.ComponentMapper.GetMapper<Pilotable>();
            _tractorables = world.ComponentMapper.GetMapper<Tractorable>();
            _nodes = world.ComponentMapper.GetMapper<Node>();
            _bodies = world.ComponentMapper.GetMapper<Body>();
            _trees = world.ComponentMapper.GetMapper<Tree>();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Update(int emitterId)
        {
            if (!_emitters.TryGet(emitterId, out TractorBeamEmitter? emitter))
            {
                return;
            }

            if(emitter.TractorBeam is null)
            {
                return;
            }

            if (!_pilotables.TryGet(emitterId, out Pilotable? pilotable))
            {
                return;
            }

            if (!_bodies.TryGet(emitterId, out Body? body))
            {
                return;
            }

            if (!_bodies.TryGet(emitter.TractorBeam.Target.EntityId, out Body? targetBody))
            {
                return;
            }

            if(!this.TryGetPotentialLink(emitterId, out Link? link))
            {
                targetBody.SetTransformIgnoreContacts(
                    position: pilotable.Aim.Value - (targetBody.WorldCenter - targetBody.Position), 
                    angle: targetBody.Rotation);

                return;
            }

            Matrix transformation = link.LocalTransformation * body.GetTransformation();

            targetBody.SetTransformIgnoreContacts(
                position: Vector2.Transform(Vector2.Zero, transformation), 
                angle: -transformation.Radians());
        }

        public bool Query(
            ISimulation simulation, 
            int emitterId, 
            out int targetNodeId)
        {
            if (!_emitters.TryGet(emitterId, out TractorBeamEmitter? emitter))
            {
                targetNodeId = default!;
                return false;
            }

            if (!_pilotables.TryGet(emitterId, out Pilotable? pilotable))
            {
                targetNodeId = default!;
                return false;
            }

            const float Radius = 5f;
            AABB aabb = new AABB(pilotable.Aim.Value, Radius, Radius);
            float minDistance = Radius;
            int? callbackTargetId = default!;

            simulation.Aether.QueryAABB(fixture =>
            {
                if (fixture.Tag is not int entityId)
                { // Invalid target - not an entity
                    return true;
                }

                if (!_nodes.TryGet(entityId, out var node))
                { // Invalid target - not a node
                    return true;
                }

                if (node.Tree is null)
                { // Invalid Target - not attached to a tree
                    return true;
                }

                if (!_tractorables.TryGet(node.Tree.EntityId, out var tractorable) &&
                    !(node.Tree.EntityId == emitter.EntityId && node.Tree.Head != node))
                { // Invalid Target - The node is neither not a tractorable, nor not attached to the current ship (excluding bridge)
                    return true;
                }

                var distance = Vector2.Distance(pilotable.Aim.Value, node.CenterWorldPosition);

                if (distance >= minDistance)
                { // The new distance is further away than the previously closest found target
                    return true;
                }

                // Update the target tractorable
                minDistance = distance;
                callbackTargetId = node.EntityId;

                return true;
            }, ref aabb);

            if (callbackTargetId is null)
            {
                targetNodeId = default;
                return false;
            }

            targetNodeId = callbackTargetId.Value;
            return true;
        }

        public bool TryGetPotentialLink(
            int emitterId,
            [MaybeNullWhen(false)] out Link link)
        {
            if (!_emitters.TryGet(emitterId, out TractorBeamEmitter? emitter))
            {
                link = default;
                return false;
            }

            if (emitter.TractorBeam is null)
            {
                link = default;
                return false;
            }

            if (!_pilotables.TryGet(emitterId, out Pilotable? pilotable))
            {
                link = default;
                return false;
            }

            if(!_trees.TryGet(emitterId, out Tree? tree))
            {
                link = default;
                return false;
            }

            if (!_bodies.TryGet(emitterId, out Body? body))
            {
                link = default;
                return false;
            }

            if(!_trees.TryGet(emitter.TractorBeam.Target.EntityId, out Tree? targetTree))
            {
                link = default;
                return false;
            }

            Joint? targetJoint = targetTree.Head?.Joints.FirstOrDefault(x => x.Link is null);
            if(targetJoint is null)
            {
                link = default;
                return false;
            }

            Matrix worldTransformation = body.GetTransformation();
            float minimumJointDistance = 1f;
            Joint? emitterJoint = default;

            // Emitter nodes are essentially the current ship's parts.
            foreach (Node node in tree.Nodes)
            {
                foreach(Joint joint in node.Joints)
                {
                    if(joint.Link is not null)
                    { // The joint is already attacked to something else
                        continue;
                    }

                    Vector2 jointPosition = Vector2.Transform(Vector2.Zero, joint.LocalTransformation * worldTransformation);
                    float jointDistance = Vector2.Distance(pilotable.Aim.Target, jointPosition);

                    if(jointDistance < minimumJointDistance)
                    {
                        minimumJointDistance = jointDistance;
                        emitterJoint = joint;
                    }
                }
            }

            if(emitterJoint is null)
            {
                link = default;
                return false;
            }

            link = new Link(targetJoint, emitterJoint);
            return true;
        }
    }
}

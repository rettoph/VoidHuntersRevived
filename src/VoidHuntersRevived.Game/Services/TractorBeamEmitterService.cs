using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Game.Components;

namespace VoidHuntersRevived.Game.Services
{
    public sealed class TractorBeamEmitterService : IQueryingEntitiesEngine
    {
        private static Fix64 QueryRadius = (Fix64)3;
        private static readonly Fix64 OpenNodemaximumDistance = Fix64.One;

        private readonly ISpace _space;
        private readonly IEntityService _entities;

        public TractorBeamEmitterService(ISpace space, IEntityService entities)
        {
            _space = space;
            _entities = entities;
            this.entitiesDB = null!;
        }

        public EntitiesDB entitiesDB { get; set; }

        public void Ready()
        {
            // throw new NotImplementedException();
        }

        public bool Query(EntityId tractorBeamEmitterId, FixVector2 target, out Component<Node> targetNode, out Component<Tree> targetTree)
        {
            if(!this.entitiesDB.TryQueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint index, out var tractorBeamEmitters))
            {
                targetNode = default;
                targetTree = default;
                return false;
            }

            TractorBeamEmitter tractorBeamEmitter = tractorBeamEmitters[index];

            AABB aabb = new AABB(target, QueryRadius, QueryRadius);
            Fix64 minDistance = QueryRadius;
            Component<Node>? callbackTargetNode = default!;
            Component<Tree>? callbackTargetTree = default!;

            _space.QueryAABB(fixture =>
            {
                VhId queryTargetId = default;

                // BEGIN NODE DISTANCE CHECK
                EntityId queryNodeId = _entities.GetId(fixture.Id);
                Node queryNode = this.entitiesDB.QueryEntitiesAndIndex<Node>(queryNodeId.EGID, out uint index)[index];
                var (rigids, _) = this.entitiesDB.QueryEntities<Rigid>(queryNodeId.EGID.groupID);
                Rigid queryRigid = rigids[index];


                FixVector2 queryNodePosition = FixVector2.Transform(queryRigid.Centeroid, queryNode.Transformation);
                FixVector2.Distance(ref target, ref queryNodePosition, out Fix64 queryNodeDistance);

                if (queryNodeDistance > minDistance)
                { // Invalid Target - The distance is further away than the previously closest valid target
                    return true;
                }

                // BEGIN NODE TREE CHECK
                Tree queryTree = this.entitiesDB.QueryEntity<Tree>(queryNode.TreeId.EGID);

                if(this.entitiesDB.TryGetEntity<Tractorable>(queryNode.TreeId.EGID, out var tractorable) && tractorable.IsTractored == false)
                { // Target resides within a tractorable tree, so we want to grab the head
                    queryTargetId = queryTree.HeadId.VhId;
                }
                else if(queryNode.TreeId.VhId == tractorBeamEmitterId.VhId && queryTree.HeadId.VhId != fixture.Id)
                {
                    queryTargetId = fixture.Id;
                }
                else
                { // Target is not in any way tractorable, we can disregard it
                    return true;
                }

                minDistance = queryNodeDistance;
                callbackTargetNode = new Component<Node>(queryNodeId, queryNode);
                callbackTargetTree = new Component<Tree>(queryNode.TreeId, queryTree);
                return true; // Ensure we only check a maximum of 5 fixtures all the way through

            }, ref aabb);

            if (callbackTargetNode is null || callbackTargetTree is null)
            {
                targetNode = default;
                targetTree = default;
                return false;
            }

            targetNode = callbackTargetNode.Value;
            targetTree = callbackTargetTree.Value;
            return true;
        }

        public bool TryGetClosestOpenSocket(EntityId shipId, FixVector2 target, [MaybeNullWhen(false)] out SocketNode socketNode)
        {
            // Since ships are Trees the ShipId will be the filterId seen in NodeEngine
            ref var filter = ref _entities.GetFilter<Node>(shipId, Tree.NodeFilterContextId);
            Fix64 closestOpenSocketDistance = OpenNodemaximumDistance;
            socketNode = default!;
            bool result = false;
        
            foreach (var (indeces, group) in filter)
            {
                if (!this.entitiesDB.HasAny<Sockets>(group))
                {
                    continue;
                }
        
                var (nodes, socketses, _) = entitiesDB.QueryEntities<Node, Sockets>(group);
        
                for (int i = 0; i < indeces.count; i++)
                {
                    uint index = indeces[i];
                    if (
                        this.TryGetClosestOpenSocketOnNode(target, ref nodes[index], ref socketses[index], out Fix64 closestOpenSocketOnNodeDistance, out var closestOpenSocketOnNode)
                        && closestOpenSocketOnNodeDistance < closestOpenSocketDistance)
                    {
                        closestOpenSocketDistance = closestOpenSocketOnNodeDistance;
                        socketNode = new SocketNode(ref closestOpenSocketOnNode.Socket, ref closestOpenSocketOnNode.Node);
                        result = true;
                    }
                }
            }
        
            return result;
        }
        
        private bool TryGetClosestOpenSocketOnNode(
            FixVector2 target,
            ref Node node,
            ref Sockets sockets,
            out Fix64 closestOpenSocketDistance,
            out SocketNode closestOpenSocketOnNode)
        {
            closestOpenSocketDistance = OpenNodemaximumDistance;
            closestOpenSocketOnNode = default!;
            bool result = false;
        
            for (int j = 0; j < sockets.Items.count; j++)
            {
                ref Socket socket = ref sockets.Items[j];

                if(socket.PlugId.VhId != default)
                { // Socket is not open
                    continue;
                }

                FixMatrix jointWorldTransformation = socket.Location.Transformation * node.Transformation;
                FixVector2 jointWorldPosition = FixVector2.Transform(FixVector2.Zero, jointWorldTransformation);
                FixVector2.Distance(ref jointWorldPosition, ref target, out Fix64 jointDistanceFromTarget);
                if (jointDistanceFromTarget > closestOpenSocketDistance)
                { // Socket is further away than previously checked closest
                    continue;
                }
        
                closestOpenSocketDistance = jointDistanceFromTarget;
                closestOpenSocketOnNode = new SocketNode(ref socket, ref node);
                result = true;
            }
        
            return result;
        }
    }
}

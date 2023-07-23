using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Game.Components;

namespace VoidHuntersRevived.Game.Services
{
    public sealed class TractorBeamEmitterService : IQueryingEntitiesEngine
    {
        private static Fix64 QueryRadius = (Fix64)5;

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

        public bool Query(EntityId tractorBeamEmitterId, FixVector2 target, out EntityId targetId)
        {
            var tractorBeamEmitters = this.entitiesDB.QueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint index);

            TractorBeamEmitter tractorBeamEmitter = tractorBeamEmitters[index];

            AABB aabb = new AABB(target, QueryRadius, QueryRadius);
            Fix64 minDistance = QueryRadius;
            VhId? callbackTargetId = default!;
            int queryCount = 0;

            _space.QueryAABB(fixture =>
            {
                VhId queryTargetId = default;

                // BEGIN NODE DISTANCE CHECK
                EntityId fixtureNodeId = _entities.GetId(fixture.Id);
                Node fixtureNode = this.entitiesDB.QueryEntity<Node>(fixtureNodeId.EGID);
                FixVector2 fixtureNodePosition = FixVector2.Transform(FixVector2.Zero, fixtureNode.Transformation);
                FixVector2.Distance(ref target, ref fixtureNodePosition, out Fix64 fixtureNodeDistance);

                if (fixtureNodeDistance < minDistance)
                { // Node is closer than a previously scanned target
                    minDistance = fixtureNodeDistance;
                }
                else
                { // Invalid Target - The distance is further away than the previously closest valid target
                    return true;
                }

                // BEGIN NODE TREE CHECK
                Tree fixtureNodeTree = this.entitiesDB.QueryEntity<Tree>(fixtureNode.TreeId.EGID);

                if(this.entitiesDB.TryGetEntity<Tractorable>(fixtureNode.TreeId.EGID, out var tractorable) && tractorable.IsTractored == false)
                { // Target resides within a tractorable tree, so we want to grab the head
                    queryTargetId = fixtureNodeTree.HeadId.VhId;
                }
                else
                { // Target is not in any way tractorable, we can disregard it
                    return true;
                }

                callbackTargetId = queryTargetId;
                return queryCount++ < 5; // Ensure we only check a maximum of 5 fixtures all the way through

            }, ref aabb);

            if (callbackTargetId is null)
            {
                targetId = default;
                return false;
            }

            targetId = _entities.GetId(callbackTargetId.Value);
            return true;
        }
    }
}

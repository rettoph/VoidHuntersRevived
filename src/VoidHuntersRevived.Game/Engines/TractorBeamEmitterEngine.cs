using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Game.Pieces.Descriptors;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterEngine : BasicEngine,
        IEventEngine<SetTractorBeamEmitterActive>
    {
        public static readonly Fix64 QueryRadius = (Fix64)5;

        public void Process(VhId eventId, SetTractorBeamEmitterActive data)
        {
            IdMap id = this.Simulation.Entities.GetIdMap(data.ShipId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(id.EGID.groupID).Entity(id.EGID.entityID);
            ref Tactical tactical = ref entitiesDB.QueryMappedEntities<Tactical>(id.EGID.groupID).Entity(id.EGID.entityID);

            if (tractorBeamEmitter.Active == data.Value)
            {
                return;
            }

            if (data.Value)
            {
                this.ActivateTractorBeamEmitter(ref eventId, ref id, ref tractorBeamEmitter, ref tactical);
            }
        }

        private void ActivateTractorBeamEmitter(ref VhId eventId, ref IdMap shipId, ref TractorBeamEmitter tractorBeamEmitter, ref Tactical tactical)
        {
            if(!this.Query(tactical.Target, QueryRadius, out VhId targetId))
            {
                return;
            }

            this.Simulation.Entities.Clone(targetId, targetId.Create(1));
        }

        public bool Query(FixVector2 target, Fix64 radius, out VhId targetId)
        {
            AABB aabb = new AABB(target, radius, radius);
            Fix64 minDistance = radius;
            VhId? callbackTargetId = default!;

            this.Simulation.Space.QueryAABB(fixture =>
            {
                IdMap bodyId = this.Simulation.Entities.GetIdMap(fixture.Body.Id);

                if(!this.entitiesDB.TryGetEntity<Tractorable>(bodyId.EGID, out _))
                { // Invalid target - not tractorable
                    return true;
                }

                FixVector2 position = fixture.Body.Position;
                FixVector2.Distance(ref target, ref position, out Fix64 distance);
                if (distance >= minDistance)
                { // Invalid Target - The distance is further away than the previously closest valid target
                    return true;
                }

                minDistance = distance;
                callbackTargetId = fixture.Body.Id;

                return true;
            }, ref aabb);

            if (callbackTargetId is null)
            {
                targetId = default;
                return false;
            }

            targetId = callbackTargetId.Value;
            return true;
        }
    }
}

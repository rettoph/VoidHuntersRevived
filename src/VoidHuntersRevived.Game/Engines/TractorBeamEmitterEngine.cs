using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Game.Pieces;
using VoidHuntersRevived.Game.Pieces.Components;
using VoidHuntersRevived.Game.Pieces.Descriptors;
using static System.Net.Mime.MediaTypeNames;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterEngine : BasicEngine,
        IEventEngine<SetTractorBeamEmitterActive>,
        IEventEngine<SetTractorBeamTarget>
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
                this.ActivateTractorBeamEmitter(eventId, ref id, ref tractorBeamEmitter, ref tactical);
            }
        }

        private void ActivateTractorBeamEmitter(VhId eventId, ref IdMap shipId, ref TractorBeamEmitter tractorBeamEmitter, ref Tactical tactical)
        {
            if(!this.Query(tactical.Target, QueryRadius, out IdMap targetId))
            {
                return;
            }

            if(!this.entitiesDB.TryGetEntity<Tree>(targetId.EGID, out Tree tree))
            {
                return;
            }

            this.Simulation.Publish(eventId.Create(tree.HeadId), new SetTractorBeamTarget()
            {
                TractorBeamId = shipId.VhId,
                TargetData = this.Simulation.Entities.Serialization.Serialize(tree.HeadId)
            });

            this.Simulation.Publish(TryDestroyEntity.NameSpace.Create(targetId.VhId), new TryDestroyEntity()
            {
                EntityVhId = targetId.VhId,
                Backup = this.Simulation.Entities.Serialization.Serialize(targetId)
            });
        }

        public void Process(VhId eventId, SetTractorBeamTarget data)
        {
            VhId targetVhId = eventId.Create(1);
            long headPosition = data.TargetData.Position;
            this.Simulation.Publish(TryCreateEntity.NameSpace.Create(targetVhId), new TryCreateEntity()
            {
                Type = EntityTypes.Chain,
                EntityVhId = targetVhId,
                Initializer = (IEntityService entities, ref EntityInitializer initializer) =>
                {
                    data.TargetData.Position = headPosition;
                    initializer.Get<Tree>().HeadId = entities.Serialization.Deserialize(eventId.Create(2), data.TargetData).VhId;
                }
            });
        }

        public bool Query(FixVector2 target, Fix64 radius, out IdMap targetId)
        {
            AABB aabb = new AABB(target, radius, radius);
            Fix64 minDistance = radius;
            IdMap? callbackTargetId = default!;

            this.Simulation.Space.QueryAABB(fixture =>
            {
                if(!this.Simulation.Entities.TryGetIdMap(fixture.Body.Id, out IdMap bodyId))
                { // Invalid target - has been deleted.
                    return true;
                }

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
                callbackTargetId = bodyId;

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

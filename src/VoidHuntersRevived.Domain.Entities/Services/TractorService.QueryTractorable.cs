using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class TractorService
    {
        public class QueryTractorable
        {
            public const float Radius = 5f;

            private readonly Pilotable _pilotable;
            private readonly TractorService _tractor;
            private float _distance;
            private int? _tractorableId;
            private ParallelKey _tractorableKey;
            private ParallelKey _nodeKey;

            private QueryTractorable(Pilotable pilotable, TractorService tractor)
            {
                _pilotable = pilotable;
                _tractor = tractor;
            }

            public bool Query(out ParallelKey tractorableKey, out ParallelKey nodeKey)
            {
                _distance = Radius;

                AABB aabb = new AABB(_pilotable.Aim.Value, Radius, Radius);
                _tractor._interactive.Aether.QueryAABB(this.QueryCallback, ref aabb);

                if (_tractorableId is null)
                {
                    tractorableKey = default;
                    nodeKey = default;
                    return false;
                }

                tractorableKey = _tractorableKey;
                nodeKey = _nodeKey;
                return true;
            }

            private bool QueryCallback(Fixture fixture)
            {
                if (fixture.Tag is not int entityId)
                { // If the fixture is not bound to an entity...
                    return true;
                }

                if (!_tractor._nodes.TryGet(entityId, out var node))
                { // If the entity is not a node...
                    return true;
                }

                if (_tractorableId == node.TreeId)
                { // This is already the target
                    return true;
                }

                if (!_tractor._tractorables.TryGet(node.TreeId, out var tractorable))
                { // If the node is not attached to a tractorable...
                    return true;
                }

                if(tractorable.WhitelistedTractoring is not null)
                { 
                    if(tractorable.WhitelistedTractoring.Value != _pilotable.EntityId)
                    { // This part is attached to another ship
                        return true;
                    }
                    
                    if(_tractor._trees.Get(_pilotable.EntityId).HeadId == node.EntityId)
                    { // We are attempting to grab the bridge
                        return true;
                    }
                }

                var distance = Vector2.Distance(_pilotable.Aim.Value, node.WorldPosition);

                if (distance >= _distance)
                { // The new distance is further away than the previously closest found target
                    return true;
                }

                // Update the target tractorable
                _distance = distance;
                _tractorableId = node.TreeId;
                _tractorableKey = _tractor._parallelables.Get(node.TreeId).Key;
                _nodeKey = _tractor._parallelables.Get(node.EntityId).Key;

                return true;
            }

            public static bool Invoke(Pilotable pilotable, TractorService tractor, out ParallelKey tractorableKey, out ParallelKey nodeKey)
            {
                var query = new QueryTractorable(pilotable, tractor);

                return query.Query(out tractorableKey, out nodeKey);
            }
        }
    }
}

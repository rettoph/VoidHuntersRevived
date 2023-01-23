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
            private Vector2 _target;
            private float _distance;
            private int? _tractorableId;
            private ParallelKey _tractorableKey;

            private QueryTractorable(Pilotable pilotable, TractorService tractor)
            {
                _pilotable = pilotable;
                _tractor = tractor;
            }

            public bool Query(out ParallelKey tractorableKey)
            {
                _distance = Radius;

                AABB aabb = new AABB(_pilotable.Aim.Value, Radius, Radius);
                _tractor._interactive.Aether.QueryAABB(this.QueryCallback, ref aabb);

                if (_tractorableId is null)
                {
                    tractorableKey = default;
                    return false;
                }

                tractorableKey = _tractorableKey;
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

                if (_tractorableId == node.Tree.Id)
                { // This is already the target
                    return true;
                }

                if (!_tractor._tractorables.Has(node.Tree.Id))
                { // If the node is not attached to a tractorable...
                    return true;
                }

                var distance = Vector2.Distance(_target, node.WorldPosition);

                if (distance >= _distance)
                { // The new distance is further away than the previously closest found target
                    return true;
                }

                // Update the target tractorable
                _distance = distance;
                _tractorableId = node.Tree.Id;
                _tractorableKey = _tractor._parallelables.Get(node.Tree.Id).Key;

                return true;
            }

            public static bool Invoke(Pilotable pilotable, TractorService tractor, out ParallelKey tractorableKey)
            {
                var query = new QueryTractorable(pilotable, tractor);

                return query.Query(out tractorableKey);
            }
        }
    }
}

using Guppy.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public static class EntityTypes
    {
        public static readonly EntityType Ship = new EntityType(nameof(Ship));
        public static readonly EntityType TractorableShipPart = new EntityType(nameof(TractorableShipPart));

        public class Base
        {
            public static readonly EntityType ShipPart = new EntityType(nameof(ShipPart));
        }
    }
}

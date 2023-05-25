using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Common.FixedPoint
{
    public struct FixAABB
    {
        /// <summary>
        /// The lower vertex
        /// </summary>
        public FixVector2 LowerBound;

        /// <summary>
        /// The upper vertex
        /// </summary>
        public FixVector2 UpperBound;

        public FixAABB(FixVector2 min, FixVector2 max)
            : this(ref min, ref max)
        {
        }

        public FixAABB(ref FixVector2 min, ref FixVector2 max)
        {
            LowerBound = min;
            UpperBound = max;
        }

        public FixAABB(FixVector2 center, Fix64 width, Fix64 height)
        {
            LowerBound = center - new FixVector2(width / (Fix64)2, height / (Fix64)2);
            UpperBound = center + new FixVector2(width / (Fix64)2, height / (Fix64)2);
        }

        public static explicit operator AABB(FixAABB aabb)
        {
            return new AABB((AetherVector2)aabb.LowerBound, (AetherVector2)aabb.UpperBound);
        }
    }
}

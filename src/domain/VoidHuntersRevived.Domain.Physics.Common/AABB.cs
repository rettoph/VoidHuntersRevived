namespace VoidHuntersRevived.Common.FixedPoint
{
    public struct AABB
    {
        /// <summary>
        /// The lower vertex
        /// </summary>
        public FixVector2 LowerBound;

        /// <summary>
        /// The upper vertex
        /// </summary>
        public FixVector2 UpperBound;

        public AABB(FixVector2 min, FixVector2 max)
            : this(ref min, ref max)
        {
        }

        public AABB(ref FixVector2 min, ref FixVector2 max)
        {
            LowerBound = min;
            UpperBound = max;
        }

        public AABB(FixVector2 center, Fix64 width, Fix64 height)
        {
            LowerBound = center - new FixVector2(width / (Fix64)2, height / (Fix64)2);
            UpperBound = center + new FixVector2(width / (Fix64)2, height / (Fix64)2);
        }
    }
}

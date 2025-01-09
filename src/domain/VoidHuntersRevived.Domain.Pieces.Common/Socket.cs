using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public struct Socket(FixTransform2D localTransform)
    {
        /// <summary>
        /// The socket location relative to the owning node
        /// </summary>
        public FixTransform2D NodeTransform { get; set; } = localTransform;
    }
}
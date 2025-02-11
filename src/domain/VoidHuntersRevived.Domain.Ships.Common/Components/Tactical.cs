using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct Tactical : IEntityComponent
    {
        public FixVector2 Value;
        public FixVector2 Target;
        public int Uses;

        public void AddUse()
        {
            this.Uses++;
            Console.WriteLine(this.Uses);
        }

        public void RemoveUse()
        {
            this.Uses = Math.Max(this.Uses - 1, 0);
            Console.WriteLine(this.Uses);
        }
    }
}
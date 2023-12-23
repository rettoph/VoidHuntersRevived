using Svelto.ECS;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct Tactical : IEntityComponent
    {
        public FixVector2 Value;
        public FixVector2 Target;
        public int Uses;

        public void AddUse()
        {
            this.Uses++;
        }

        public void RemoveUse()
        {
            this.Uses = Math.Max(--this.Uses, 0);
        }
    }
}

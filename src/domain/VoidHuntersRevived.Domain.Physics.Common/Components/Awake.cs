using Svelto.ECS;

namespace VoidHuntersRevived.Common.Physics.Components
{
    public struct Awake : IEntityComponent
    {
        public readonly bool SleepingAllowed;
        public bool Value;

        public Awake(bool sleepingAllowed = true) : this()
        {
            this.SleepingAllowed = sleepingAllowed;
            this.Value = true;
        }

        public static implicit operator bool(Awake obj) => obj.Value;
    }
}

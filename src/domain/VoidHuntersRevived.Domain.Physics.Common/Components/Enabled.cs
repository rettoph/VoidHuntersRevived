using Svelto.ECS;

namespace VoidHuntersRevived.Common.Physics.Components
{
    public struct Enabled : IEntityComponent
    {
        public bool Value;

        public static implicit operator bool(Enabled obj) => obj.Value;
    }
}

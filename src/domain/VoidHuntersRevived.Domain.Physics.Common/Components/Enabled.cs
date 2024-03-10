using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public struct Enabled : IEntityComponent
    {
        public bool Value;

        public static implicit operator bool(Enabled obj) => obj.Value;
    }
}

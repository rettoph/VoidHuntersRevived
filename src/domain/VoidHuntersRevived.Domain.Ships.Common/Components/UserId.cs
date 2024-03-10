using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct UserId : IEntityComponent
    {
        public UserId(int value)
        {
            this.Value = value;
        }

        public int? Value { get; internal set; }
    }
}

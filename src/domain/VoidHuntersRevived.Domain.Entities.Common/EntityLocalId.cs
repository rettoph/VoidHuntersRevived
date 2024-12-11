using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct EntityLocalId(EGID egid) : IEntityComponent
    {
        public readonly EGID Value = egid;

        public ExclusiveGroupStruct Group => this.Value.groupID;

        public override bool Equals(object? obj)
        {
            return obj is EntityLocalId id &&
                   Value.Equals(id.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}

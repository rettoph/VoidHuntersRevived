using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct EntityLocalId(EGID egid) : IEntityComponent
    {
        public readonly EGID Value = egid;

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}

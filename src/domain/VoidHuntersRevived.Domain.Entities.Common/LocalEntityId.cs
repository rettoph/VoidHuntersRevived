using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct LocalEntityId(EGID egid) : IEntityComponent
    {
        public readonly EGID Value = egid;
    }
}

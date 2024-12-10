using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct LocalId(EGID egid)
    {
        public readonly EGID Value = egid;
    }
}

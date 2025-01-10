using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Extensions.Svelto.ECS
{
    public static class EGIDExtensions
    {
        public static EntityLocalId ToEntityLocalId(this EGID egid) => new(egid);
    }
}
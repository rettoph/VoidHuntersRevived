using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct GlobalEntityId(VhId vhid) : IEntityComponent
    {
        public readonly VhId Value = vhid;
    }
}

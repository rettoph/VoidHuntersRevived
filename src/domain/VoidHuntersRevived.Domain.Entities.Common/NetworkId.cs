using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct NetworkId(VhId vhid)
    {
        public readonly VhId Value = vhid;
    }
}

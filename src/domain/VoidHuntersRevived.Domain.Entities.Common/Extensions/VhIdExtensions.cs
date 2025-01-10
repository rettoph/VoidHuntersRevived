using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Extensions
{
    public static class VhIdExtensions
    {
        public static EntityGlobalId ToGlobalEntityId(this VhId vhid)
        {
            return new(vhid);
        }

        public static EntityGlobalId ToGlobalEntityId(this VhId vhid, int name)
        {
            return new(vhid.Create(name));
        }
    }
}
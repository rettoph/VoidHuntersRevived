using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Extensions
{
    public static class VhIdExtensions
    {
        public static EntityGlobalId ToGlobalEntityId(this VhId vhid)
        {
            return new EntityGlobalId(vhid);
        }

        public static EntityGlobalId ToGlobalEntityId(this VhId vhid, int name)
        {
            return new EntityGlobalId(vhid.Create(name));
        }
    }
}

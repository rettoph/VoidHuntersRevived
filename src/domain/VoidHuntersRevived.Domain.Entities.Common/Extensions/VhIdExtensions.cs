using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Extensions
{
    public static class VhIdExtensions
    {
        public static GlobalEntityId ToGlobalEntityId(this VhId vhid)
        {
            return new GlobalEntityId(vhid);
        }

        public static GlobalEntityId ToGlobalEntityId(this VhId vhid, int name)
        {
            return new GlobalEntityId(vhid.Create(name));
        }
    }
}

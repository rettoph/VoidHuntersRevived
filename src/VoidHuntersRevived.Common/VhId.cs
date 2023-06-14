using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common
{
    public struct VhId
    {
        public readonly Guid Value;

        public VhId(string g)
        {
            this.Value = new Guid(g);
        }
        public VhId(Guid guid)
        {
            this.Value = guid;
        }
        public static VhId NewId()
        {
            return new VhId(Guid.NewGuid());
        }
    }
}

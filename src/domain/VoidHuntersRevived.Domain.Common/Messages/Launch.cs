using Guppy;
using Guppy.Messaging;

namespace VoidHuntersRevived.Domain.Common.Messages
{
    public class Launch : Message<Launch>
    {
        public readonly Type GuppyType;

        internal Launch(Type guppyType)
        {
            this.GuppyType = guppyType;
        }
    }

    public class Launch<TGuppy> : Launch
        where TGuppy : IGuppy
    {
        public static readonly Launch<TGuppy> Instance = new Launch<TGuppy>();

        private Launch() : base(typeof(TGuppy))
        {
        }
    }
}

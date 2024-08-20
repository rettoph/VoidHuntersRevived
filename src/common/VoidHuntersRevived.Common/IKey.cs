using Guppy.Core.Common.Utilities;

namespace VoidHuntersRevived.Common
{
    public interface IKey : IEquatable<IKey>
    {
        string Name { get; }

        VhId Id { get; }
        Type Type { get; }
    }

    public interface IKey<out T> : IKey
    {
        UnmanagedReference<IKey<TRef>> AsRef<TRef>();
    }
}

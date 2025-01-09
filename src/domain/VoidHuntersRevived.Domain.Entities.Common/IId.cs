using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IId<out T>
    {
        VhId Value { get; }
    }
}
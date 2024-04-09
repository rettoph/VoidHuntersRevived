namespace VoidHuntersRevived.Common.Entities
{
    public interface IId<out T>
    {
        VhId Value { get; }
    }
}

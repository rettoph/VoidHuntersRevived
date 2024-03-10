namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityResource<T>
    {
        Id<T> Id { get; }
    }
}

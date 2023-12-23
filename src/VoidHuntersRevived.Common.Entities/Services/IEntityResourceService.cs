namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityResourceService<T>
        where T : IEntityResource<T>
    {
        T GetById(Id<T> id);

        IEnumerable<T> GetAll();
    }
}

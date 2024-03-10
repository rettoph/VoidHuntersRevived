namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityTypeService
    {
        IEntityType GetById(Id<IEntityType> id);
    }
}

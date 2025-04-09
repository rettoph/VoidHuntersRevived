using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityAssetService<T>
        where T : IEntityAsset<T>
    {
        T GetById(Id<T> id);

        IEnumerable<T> GetAll();
    }
}
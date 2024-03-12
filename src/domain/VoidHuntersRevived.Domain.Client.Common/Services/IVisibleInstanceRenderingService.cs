using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Client.Common.Services
{
    public interface IVisibleInstanceRenderingService
    {
        void Begin();
        void Draw(Id<IEntityType> entityTypeId, int count, ref EntityFilterCollection instances);
        void End();
    }
}

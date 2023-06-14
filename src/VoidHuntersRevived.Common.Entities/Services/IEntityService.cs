using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public delegate void EntityInitializerDelegate(ref EntityInitializer initializer);

    public interface IEntityService
    {
        VhId Create(EntityName name, VhId id);
        VhId Create(EntityName name, VhId id, EntityInitializerDelegate initializer);

        T GetProperty<T>(Property<T> id) 
            where T : class, IEntityProperty;

        EGID GetEGID(VhId id);
        VhId GetVhId(EGID egid);

        void Destroy(VhId id);
    }
}

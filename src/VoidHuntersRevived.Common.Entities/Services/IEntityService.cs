using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public delegate void EntityInitializerDelegate(ref EntityInitializer initializer);

    public interface IEntityService
    {
        VhId Create(EntityType type, VhId id);
        VhId Create(EntityType type, VhId id, EntityInitializerDelegate initializer);

        EGID GetEGID(VhId id);
        VhId GetVhId(EGID egid);
        VhId GetVhId(uint id, ExclusiveGroupStruct group);

        void Destroy(VhId id);
    }
}

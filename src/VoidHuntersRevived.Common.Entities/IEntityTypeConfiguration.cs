using Svelto.ECS;
using Svelto.ECS.Serialization;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeConfiguration
    {
        public EntityType Type { get; }

        void Initialize(IWorld world, ref EntityInitializer initializer);

        IEntityTypeConfiguration HasInitializer(EntityInitializerDelegate initializer);
    }
}

using Svelto.ECS;
using Svelto.ECS.Serialization;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeConfiguration
    {
        public EntityType Type { get; }

        void Initialize(IEngineService engines, ref EntityInitializer initializer);

        IEntityTypeConfiguration HasInitializer(EntityInitializerDelegate initializer);
    }
}

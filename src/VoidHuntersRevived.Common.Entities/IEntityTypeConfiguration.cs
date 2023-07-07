using Svelto.ECS;
using Svelto.ECS.Serialization;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeConfiguration
    {
        public IEntityType Type { get; }

        void Initialize(ref EntityInitializer initializer);

        IEntityTypeConfiguration HasInitializer(EntityInitializerDelegate initializer);
    }
}

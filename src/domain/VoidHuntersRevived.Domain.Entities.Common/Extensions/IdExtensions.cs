using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Extensions
{
    public static class IdExtensions
    {
        public static EntityInitializerDelegate EntityInitializer<T>(this Id<T> value)
        {
            return (IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer) =>
            {
                // TODO: Only call this if necessary? 
                initializer.Init(value);
            };
        }
    }
}

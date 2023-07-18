using Guppy.Resources;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct EntityResource<T> : IEntityComponent
    {
        public readonly Guid Value;

        public EntityResource(Guid value)
        {
            this.Value = value;
        }

        public static implicit operator EntityResource<T>(Resource<T> resource)
        {
            return new EntityResource<T>(resource.Id);
        }
    }
}

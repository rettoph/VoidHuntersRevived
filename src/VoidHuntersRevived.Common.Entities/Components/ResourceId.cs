using Guppy.Resources;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct ResourceId<T> : IEntityComponent
        where T : notnull
    {
        public readonly Guid Value;

        public ResourceId(Guid value)
        {
            this.Value = value;
        }

        public static implicit operator ResourceId<T>(Resource<T> resource)
        {
            return new ResourceId<T>(resource.Id);
        }
    }
}

using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public readonly struct ResourceComponent<T>(Resource<T> value) : IEntityComponent
        where T : notnull
    {
        private readonly Resource<T> _value = value;

        public ResourceKey<T> Resource => _value.Key;
        public T Value => _value.Value;

        public ResourceComponent(string name, IResourceService resourceService) : this(resourceService.Get(ResourceKey<T>.Get(name)))
        {
        }
    }
}

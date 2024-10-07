using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public readonly struct ResourceComponent<T>(ResourceValue<T> value) : IEntityComponent
        where T : notnull
    {
        private readonly ResourceValue<T> _value = value;

        public Resource<T> Resource => _value.Resource;
        public T Value => _value.Value;

        public ResourceComponent(string name, IResourceService resourceService) : this(resourceService.GetValue(Resource<T>.Get(name)))
        {
        }
    }
}

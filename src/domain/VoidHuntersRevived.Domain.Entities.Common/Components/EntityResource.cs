using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct EntityResource<TResource> : IEntityComponent
        where TResource : IEntityResource<TResource>
    {
        public readonly Id<TResource> Id;

        public EntityResource(Id<TResource> value)
        {
            this.Id = value;
        }
    }

    public struct EntityResource<TResource, TInstance> : IEntityComponent
        where TResource : IEntityResource<TResource>
        where TInstance : unmanaged
    {
        public readonly Id<TResource> Id;
        public TInstance Instance;

        public EntityResource(Id<TResource> id, TInstance instance)
        {
            this.Id = id;
            this.Instance = instance;
        }

        public EntityResource(Id<TResource> id) : this(id, default)
        {

        }
    }
}

using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public abstract class EntityType : IEntityType
    {
        private static List<EntityType> _list = new List<EntityType>();

        public readonly Id<IEntityType> Id;
        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly string Key;

        Id<IEntityType> IEntityResource<IEntityType>.Id => this.Id;
        VoidHuntersEntityDescriptor IEntityType.Descriptor => this.Descriptor;
        string IEntityType.Key => this.Key;

        internal unsafe EntityType(VhId nameSpace, string key, VoidHuntersEntityDescriptor descriptor)
        {
            this.Key = key;

            this.Id = new Id<IEntityType>(nameSpace.Create(key));

            _list.Add(this);
            this.Descriptor = descriptor;
        }

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }
    }

    public sealed class EntityType<TDescriptor> : EntityType, IEntityType<TDescriptor>
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        public readonly new TDescriptor Descriptor;

        TDescriptor IEntityType<TDescriptor>.Descriptor => this.Descriptor;

        public EntityType(string key) : base(NameSpace<TDescriptor>.Instance, key, new TDescriptor())
        {
            this.Descriptor = new TDescriptor();
        }
    }
}

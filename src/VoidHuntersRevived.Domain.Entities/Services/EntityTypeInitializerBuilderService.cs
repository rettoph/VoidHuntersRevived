using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Initializers;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeInitializerBuilderService : IEntityTypeInitializerBuilderService
    {
        private HashSet<IEntityType> _types = new HashSet<IEntityType>();
        private Dictionary<IEntityType, EntityTypeEntityInitializerBuilder> _typeInitializers = new Dictionary<IEntityType, EntityTypeEntityInitializerBuilder>();
        private Dictionary<Type, DescriptorEntityInitializerBuilder> _descriptorInitializers = new Dictionary<Type, DescriptorEntityInitializerBuilder>();

        public void Register(params IEntityType[] types)
        {
            foreach (IEntityType type in types)
            {
                _types.Add(type);
            }
        }

        public void Configure<TDescriptor>(Action<IEntityInitializerBuilder> builderDelegate)
            where TDescriptor : VoidHuntersEntityDescriptor
        {
            ref DescriptorEntityInitializerBuilder? builder = ref CollectionsMarshal.GetValueRefOrAddDefault(_descriptorInitializers, typeof(TDescriptor), out bool exists);
            if (exists == false)
            {
                builder = new DescriptorEntityInitializerBuilder(typeof(TDescriptor));
            }

            builderDelegate(builder!);
        }

        public void Configure(IEntityType type, Action<IEntityInitializerBuilder> builderDelegate)
        {
            ref EntityTypeEntityInitializerBuilder? builder = ref CollectionsMarshal.GetValueRefOrAddDefault(_typeInitializers, type, out bool exists);
            if (exists == false)
            {
                builder = new EntityTypeEntityInitializerBuilder(type);
            }

            builderDelegate(builder!);
        }

        public Dictionary<IEntityType, IEntityTypeInitializer> Build()
        {
            return _types.Concat(_typeInitializers.Keys).Distinct().ToDictionary(
                keySelector: entityType => entityType,
                elementSelector: entityType =>
                {
                    IEnumerable<BaseEntityInitializerBuilder> typeInitializers = _typeInitializers.Values.Where(x => x.Type == entityType);
                    IEnumerable<BaseEntityInitializerBuilder> descriptorInitializers = _descriptorInitializers.Values.Where(x => x.DescriptorType.IsAssignableFrom(entityType.Descriptor.GetType()));

                    IEntityTypeInitializer result = new EntityTypeInitializer(entityType, typeInitializers.Concat(descriptorInitializers));

                    return result;
                });
        }
    }
}

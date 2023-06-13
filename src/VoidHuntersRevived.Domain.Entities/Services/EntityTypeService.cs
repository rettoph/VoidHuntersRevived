using Guppy.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Abstractions;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeService : IEntityTypeService
    {
        private Dictionary<EntityType, EntityTypeConfiguration> _configurations;
        private Dictionary<EntityType, EntityDescriptorGroup> _descriptorGroups;

        public EntityTypeService(ISorted<IEntityTypeLoader> loaders)
        {
            _configurations = new Dictionary<EntityType, EntityTypeConfiguration>();

            foreach (IEntityTypeLoader loader in loaders)
            {
                loader.Configure(this);
            }

            _descriptorGroups = _configurations.Values.ToDictionary(
                keySelector: x => x.Type,
                elementSelector: x => x.BuildEntityDescriptorGroup(this));
        }

        public void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration)
        {
            configuration(this.GetConfiguration(type));
        }

        internal EntityDescriptorGroup EntityDescriptorGroup(EntityType type)
        {
            return _descriptorGroups[type];
        }

        internal EntityTypeConfiguration GetConfiguration(EntityType type)
        {
            if(!_configurations.TryGetValue(type, out EntityTypeConfiguration? configuration))
            {
                _configurations[type] = configuration = new EntityTypeConfiguration(type);
            }

            return configuration;
        }
    }
}

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

        public EntityTypeService(ISorted<IEntityTypeLoader> loaders)
        {
            _configurations = new Dictionary<EntityType, EntityTypeConfiguration>();

            foreach (IEntityTypeLoader loader in loaders)
            {
                loader.Configure(this);
            }

            foreach(EntityTypeConfiguration configuration in _configurations.Values)
            {
                configuration.Initialize(this);
            }
        }

        public void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration)
        {
            configuration(this.GetOrCreateConfiguration(type));
        }

        internal EntityTypeConfiguration GetOrCreateConfiguration(EntityType type)
        {
            if(!_configurations.TryGetValue(type, out EntityTypeConfiguration? configuration))
            {
                _configurations[type] = configuration = new EntityTypeConfiguration(type);
            }

            return configuration;
        }

        internal EntityTypeConfiguration GetConfiguration(EntityType type)
        {
            return _configurations[type];
        }
    }
}

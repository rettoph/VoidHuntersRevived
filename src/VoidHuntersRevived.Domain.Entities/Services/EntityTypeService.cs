using Guppy.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Abstractions;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public sealed class EntityTypeService : IEntityTypeService
    {
        private readonly Dictionary<EntityType, IEntityTypeConfiguration> _configurations;

        public EntityTypeService(ISorted<IEntityTypeLoader> loaders)
        {
            _configurations = new Dictionary<EntityType, IEntityTypeConfiguration>();

            foreach (IEntityTypeLoader loader in loaders)
            {
                loader.Configure(this);
            }
        }

        public void Register(params EntityType[] types)
        {
            foreach(EntityType type in types)
            {
                this.GetOrCreateConfiguration(type);
            }
        }

        public void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration)
        {
            configuration(this.GetOrCreateConfiguration(type));
        }

        internal IEntityTypeConfiguration GetOrCreateConfiguration(EntityType type)
        {
            if(!_configurations.TryGetValue(type, out IEntityTypeConfiguration? configuration))
            {
                _configurations[type] = configuration = type.BuildConfiguration();
            }

            return configuration;
        }

        internal IEntityTypeConfiguration GetConfiguration(EntityType type)
        {
            return _configurations[type];
        }

        public IEnumerable<IEntityTypeConfiguration> GetAllConfigurations()
        {
            return _configurations.Values;
        }
    }
}

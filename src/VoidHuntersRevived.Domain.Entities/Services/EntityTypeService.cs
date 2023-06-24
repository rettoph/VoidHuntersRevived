using Guppy.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Abstractions;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public sealed class EntityTypeService : IEntityTypeService
    {
        private readonly Dictionary<EntityType, IEntityTypeConfiguration> _configurations;
        private readonly Dictionary<VhId, EntityType> _ids;

        public EntityTypeService(ISorted<IEntityTypeLoader> loaders)
        {
            _configurations = new Dictionary<EntityType, IEntityTypeConfiguration>();
            _ids = new Dictionary<VhId, EntityType>();

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
                _ids.Add(type.Id, type);
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

        public EntityType GetById(VhId id)
        {
            return _ids[id];
        }
    }
}

using Guppy.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public sealed class EntityTypeService : IEntityTypeService
    {
        private readonly Dictionary<IEntityType, IEntityTypeConfiguration> _configurations;
        private readonly Dictionary<EntityTypeId, IEntityType> _ids;

        public EntityTypeService(IFiltered<IEntityTypeLoader> loaders)
        {
            _configurations = new Dictionary<IEntityType, IEntityTypeConfiguration>();
            _ids = new Dictionary<EntityTypeId, IEntityType>();

            foreach (IEntityTypeLoader loader in loaders.Instances)
            {
                loader.Configure(this);
            }
        }

        public void Register(params IEntityType[] types)
        {
            foreach(EntityType type in types)
            {
                this.GetOrCreateConfiguration(type);
            }
        }

        public void Configure(IEntityType type, Action<IEntityTypeConfiguration> configuration)
        {
            configuration(this.GetOrCreateConfiguration(type));
        }

        internal IEntityTypeConfiguration GetOrCreateConfiguration(IEntityType type)
        {
            if(!_configurations.TryGetValue(type, out IEntityTypeConfiguration? configuration))
            {
                _configurations[type] = configuration = type.BuildConfiguration();
                _ids.Add(type.Id, type);
            }

            return configuration;
        }

        internal IEntityTypeConfiguration GetConfiguration(IEntityType type)
        {
            return _configurations[type];
        }

        public IEnumerable<IEntityTypeConfiguration> GetAllConfigurations()
        {
            return _configurations.Values;
        }

        public IEntityType GetById(EntityTypeId id)
        {
            return _ids[id];
        }
    }
}

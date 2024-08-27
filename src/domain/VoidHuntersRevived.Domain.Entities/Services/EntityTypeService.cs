using Guppy.Core.Common;
using Guppy.Core.Resources.Common.Services;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTypeService : IEntityTypeService
    {
        private readonly Dictionary<IKey<IEntityType>, IEntityType> _entityTypes;

        public EntityTypeService(
            IFiltered<EntityTypeConfiguration> entityTypeConfigurations,
            IFiltered<IEntityType> entityTypes,
            IResourceService resources)
        {
            Dictionary<string, EntityTypeConfiguration[]> entityTypeConfigurationDictionary = entityTypeConfigurations
                .Concat(resources.GetValues<EntityTypeConfiguration>().Select(x => x.Value))
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x.ToArray());

            Dictionary<string, IEntityType> entityTypesDictionary = entityTypes.ToDictionary(x => x.Key.Name, x => x);
            foreach (string key in entityTypeConfigurationDictionary.Keys)
            {
                EntityTypeService.TryCreateOrConfigureEntityType(key, entityTypeConfigurationDictionary, entityTypesDictionary);
            }

            _entityTypes = entityTypesDictionary.Values.ToDictionary(x => x.Key, x => x);
        }

        public IEnumerable<IEntityType> GetAll()
        {
            return _entityTypes.Values;
        }

        public IEntityType GetByKey(IKey<IEntityType> key)
        {
            return _entityTypes[key];
        }

        private static void TryCreateOrConfigureEntityType(
            string name,
            Dictionary<string, EntityTypeConfiguration[]> configurations,
            Dictionary<string, IEntityType> types)
        {
            if (EntityTypeConfiguration.CombineConfigurations(name, configurations, out EntityTypeConfiguration? configuration) == false)
            {
                return;
            }

            ref IEntityType? type = ref CollectionsMarshal.GetValueRefOrAddDefault(types, name, out bool exists);
            if (exists == false)
            {
                // Attempt to create a new entity type...
                Type typeType = configuration.Type ?? typeof(EntityType);
                IKey<IEntityType> typeKey = Key.GetByName<IEntityType>(configuration.Name, typeType);
                type = (IEntityType)(Activator.CreateInstance(typeType, [typeKey]) ?? throw new NotImplementedException());
            }

            // Configure type...
            ThrowIf.Type.IsNotAssignableFrom(configuration.Type ?? typeof(IEntityType), type!.GetType());

            type.WithComponents(configuration.Components.Values)
                .RequireComponents(configuration.RequiredComponents);
        }
    }
}

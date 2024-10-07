using Guppy.Core.Common;
using Guppy.Core.Resources.Common.Services;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTypeService : IEntityTypeService
    {
        private readonly Dictionary<Key<IEntityType>, IEntityType> _entityTypes;
        private readonly Dictionary<Type, object> _entityTypesByType;
        private readonly Type[] _distinctComponentTypes;

        public EntityTypeService(
            IFiltered<EntityTypeConfiguration> entityTypeConfigurations,
            IFiltered<IEntityType> entityTypes,
            IResourceService resources)
        {
            Dictionary<Key<IEntityType>, EntityTypeConfiguration[]> entityTypeConfigurationDictionary = entityTypeConfigurations
                .Concat(resources.GetValues<EntityTypeConfiguration>().Select(x => x.Value))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.ToArray());

            Dictionary<Key<IEntityType>, IEntityType> entityTypesDictionary = entityTypes.ToDictionary(x => x.Key, x => x);
            foreach (Key<IEntityType> key in entityTypeConfigurationDictionary.Keys)
            {
                EntityTypeService.TryCreateOrConfigureEntityType(key, entityTypeConfigurationDictionary, entityTypesDictionary);
            }

            _entityTypes = entityTypesDictionary.Values.ToDictionary(x => x.Key, x => x);
            _entityTypesByType = [];
            _distinctComponentTypes = _entityTypes.SelectMany(x => x.Value.Components.Keys).Distinct().ToArray();
        }

        public virtual IEnumerable<IEntityType> GetAll()
        {
            return _entityTypes.Values;
        }

        public virtual T[] GetAll<T>()
            where T : IEntityType
        {
            ref object? entityTypes = ref CollectionsMarshal.GetValueRefOrAddDefault(_entityTypesByType, typeof(T), out bool exists);
            if (exists == false)
            {
                entityTypes = _entityTypes.Values.OfType<T>().ToArray();
            }

            return (T[])entityTypes!;
        }

        public virtual IEntityType GetByKey(Key<IEntityType> key)
        {
            return _entityTypes[key];
        }

        public virtual Type[] GetAllDistinctComponentTypes()
        {
            return _distinctComponentTypes;
        }

        private static void TryCreateOrConfigureEntityType(
            Key<IEntityType> key,
            Dictionary<Key<IEntityType>, EntityTypeConfiguration[]> configurations,
            Dictionary<Key<IEntityType>, IEntityType> types)
        {
            if (EntityTypeConfiguration.CombineConfigurations(key, configurations, out EntityTypeConfiguration? configuration) == false)
            {
                return;
            }

            ref IEntityType? type = ref CollectionsMarshal.GetValueRefOrAddDefault(types, key, out bool exists);

            try
            {
                if (exists == false)
                {
                    // Attempt to create a new entity type...
                    Type typeType = configuration.Type ?? typeof(BaseEntityType);

                    type = (IEntityType)(Activator.CreateInstance(typeType, [key]) ?? throw new NotImplementedException());
                }

                // Configure type...
                ThrowIf.Type.IsNotAssignableFrom(configuration.Type ?? typeof(IEntityType), type!.GetType());

                type.WithComponents(configuration.Components.Values)
                    .RequireComponents(configuration.RequiredComponents);

                type.Verify();
            }
            catch (Exception ex)
            {
                types.Remove(key);
                throw new EntityTypeException(key, $"Exception configuring entity type", ex);
            }
        }
    }
}

using Guppy.Core.Common;
using Guppy.Core.Resources.Common.Services;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTemplateService : IEntityTemplateService
    {
        private readonly Dictionary<Key<IEntityTemplate>, IEntityTemplate> _entityTemplates;
        private readonly Dictionary<Type, object> _entityTemplatesByType;
        private readonly Type[] _distinctComponentTypes;

        public EntityTemplateService(
            IFiltered<EntityTemplateConfiguration> entityTemplateConfigurations,
            IFiltered<IEntityTemplate> entityTemplates,
            IResourceService resources)
        {
            Dictionary<Key<IEntityTemplate>, EntityTemplateConfiguration[]> entityTemplateConfigurationDictionary = entityTemplateConfigurations
                .Concat(resources.GetValues<EntityTemplateConfiguration>().Select(x => x.Value))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.ToArray());

            Dictionary<Key<IEntityTemplate>, IEntityTemplate> entityTemplatesDictionary = entityTemplates.ToDictionary(x => x.Key, x => x);
            foreach (Key<IEntityTemplate> key in entityTemplateConfigurationDictionary.Keys)
            {
                EntityTemplateService.TryCreateOrConfigureEntityTemplate(key, entityTemplateConfigurationDictionary, entityTemplatesDictionary);
            }

            _entityTemplates = entityTemplatesDictionary.Values.ToDictionary(x => x.Key, x => x);
            _entityTemplatesByType = [];
            _distinctComponentTypes = _entityTemplates.SelectMany(x => x.Value.Components.Keys).Distinct().ToArray();
        }

        public virtual IEnumerable<IEntityTemplate> GetAll()
        {
            return _entityTemplates.Values;
        }

        public virtual T[] GetAll<T>()
            where T : IEntityTemplate
        {
            ref object? entityTemplates = ref CollectionsMarshal.GetValueRefOrAddDefault(_entityTemplatesByType, typeof(T), out bool exists);
            if (exists == false)
            {
                entityTemplates = _entityTemplates.Values.OfType<T>().ToArray();
            }

            return (T[])entityTemplates!;
        }

        public virtual IEntityTemplate GetByKey(Key<IEntityTemplate> key)
        {
            return _entityTemplates[key];
        }

        public virtual Type[] GetAllDistinctComponentTypes()
        {
            return _distinctComponentTypes;
        }

        private static void TryCreateOrConfigureEntityTemplate(
            Key<IEntityTemplate> key,
            Dictionary<Key<IEntityTemplate>, EntityTemplateConfiguration[]> configurations,
            Dictionary<Key<IEntityTemplate>, IEntityTemplate> types)
        {
            if (EntityTemplateConfiguration.CombineConfigurations(key, configurations, out EntityTemplateConfiguration? configuration) == false)
            {
                return;
            }

            ref IEntityTemplate? type = ref CollectionsMarshal.GetValueRefOrAddDefault(types, key, out bool exists);

            try
            {
                if (exists == false)
                {
                    // Attempt to create a new entity type...
                    Type typeType = configuration.Type ?? typeof(BaseEntityTemplate);

                    type = (IEntityTemplate)(Activator.CreateInstance(typeType, [key]) ?? throw new NotImplementedException());
                }

                // Configure type...
                ThrowIf.Type.IsNotAssignableFrom(configuration.Type ?? typeof(IEntityTemplate), type!.GetType());

                type.WithComponents(configuration.Components.Values)
                    .RequireComponents(configuration.RequiredComponents);

                type.Verify();
            }
            catch (Exception ex)
            {
                types.Remove(key);
                throw new EntityTemplateException(key, $"Exception configuring entity type", ex);
            }
        }
    }
}

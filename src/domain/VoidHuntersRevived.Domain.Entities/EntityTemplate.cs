using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.System;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Entities
{
    internal sealed class EntityTemplate : IEntityTemplate
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly ActionSequenceGroup<OnDespawnSequenceGroupEnum, VhId, IEntityTemplate, EntityId, GroupIndex> _onDespawnEngineInvokers;
        private readonly ActionSequenceGroup<OnSpawnSequenceGroupEnum, VhId, IEntityTemplate, EntityId, GroupIndex> _onSpawnEngineInvokers;
        private EntitiesDB _entitiesDB;
        private FasterList<IComponentSerializer> _serializers;

        private DynamicEntityDescriptor<VoidHuntersEntityDescriptor> _descriptor;
        private readonly EntityGroup _group;

        public ComponentBuilderDictionary Components { get; }

        public Key<IEntityTemplate> Key { get; }

        public EntityTemplate(
            Key<IEntityTemplate> key,
            IEntityTemplateFragmentService entityTemplateService,
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityFactory factory,
            IEntityFunctions functions
        )
        {
            _uniqueNumberProvider = uniqueNumberProvider;
            _factory = factory;
            _functions = functions;
            _entitiesDB = null!;

            _onDespawnEngineInvokers = new(false);
            _onSpawnEngineInvokers = new(false);
            _serializers = null!;

            _descriptor = BuildDescriptor(key, entityTemplateService, out var components, out _group);

            this.Key = key;
            this.Components = components;
        }

        public void Initialize(
            EntitiesDB entitiesDB,
            IEngineService engineService,
            IComponentSerializerService componentSerializerService)
        {
            _entitiesDB = entitiesDB;

            // Load serializers
            _serializers = componentSerializerService.GetComponentSerializersByDescriptor(_descriptor);

            // Generate despawn engine invokers
            // Responsible for calling IOnSpawnEngine & IOnDespawnEngine engines
            List<ComponentEngineInvoker> onDespawnEngineInvokers = [];
            List<ComponentEngineInvoker> onSpawnEngineInvokers = [];

            foreach (Type componentType in this.Components.Keys)
            {
                onDespawnEngineInvokers.AddRange(ComponentEngineInvoker.Create(typeof(OnDespawnEngineInvoker<>), typeof(IOnDespawnEngine<>), componentType, engineService, _entitiesDB));
                onSpawnEngineInvokers.AddRange(ComponentEngineInvoker.Create(typeof(OnSpawnEngineInvoker<>), typeof(IOnSpawnEngine<>), componentType, engineService, _entitiesDB));
            }

            _onDespawnEngineInvokers.Add(onDespawnEngineInvokers);
            _onSpawnEngineInvokers.Add(onSpawnEngineInvokers);
        }

        #region Instance Entity Methods
        public EntityInitializer HardSpawnInstanceEntity(in VhId sourceEventId, in VhId vhid, out EntityId id)
        {
            // Create a new EGID for the entity
            EGID egid = new(_uniqueNumberProvider.GetUInt32(), _group.Value);
            id = new EntityId(egid, vhid);

            // Invoke Svelto factory and initialize instance with common component values
            EntityInitializer initializer = _factory.BuildEntity(egid, _descriptor);
            initializer.Init(id);
            initializer.Init(new EntityStatus(EntityStatusEnum.HardSpawned));

            return initializer;
        }

        public void SoftSpawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            _onSpawnEngineInvokers.Invoke(sourceEventId, this, id, groupIndex);
        }

        public void SoftDespawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            _onDespawnEngineInvokers.Invoke(sourceEventId, this, id, groupIndex);
        }

        public void HardDespawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status)
        {
            _functions.RemoveEntity<VoidHuntersEntityDescriptor>(id.EGID);
        }

        public void SerializeInstanceEntity(ref EntityWriter writer, in EntityId id, in GroupIndex groupIndex, in SerializationOptions options)
        {
            foreach (IComponentSerializer serializer in _serializers)
            {
                serializer.Serialize(ref writer, in id, in groupIndex, _entitiesDB, in options);
            }
        }

        public void DeserializeInstanceEntity(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (IComponentSerializer serializer in _serializers)
            {
                serializer.Deserialize(in sourceId, in options, ref reader, ref initializer, in id);
            }
        }
        #endregion

        public IEnumerable<Type> GetAllDistinctComponentTypes()
        {
            return _descriptor.componentsToBuild.Select(x => x.GetEntityComponentType())
                .Distinct();
        }

        private static DynamicEntityDescriptor<VoidHuntersEntityDescriptor> BuildDescriptor(
            Key<IEntityTemplate> key,
            IEntityTemplateFragmentService entityTemplateService,
            out ComponentBuilderDictionary components,
            out EntityGroup group)
        {
            components = new();
            HashSet<Type> requiredComponents = [];
            Queue<Key<IEntityTemplate>> enqueuedFragments = [];
            HashSet<Key<IEntityTemplate>> populatedTemplateKeys = [];

            // Register default components...
            components.Set(new EntityId());
            components.Set(new EntityStatus());
            components.Set(new Common.Components.EntityTemplate(key));

            enqueuedFragments.Enqueue(key);
            while (enqueuedFragments.TryDequeue(out var enqueuedTemplate) == true)
            {
                PopulateComponentCollections(
                    enqueuedTemplate,
                    entityTemplateService,
                    ref components,
                    ref requiredComponents,
                    ref enqueuedFragments,
                    ref populatedTemplateKeys);
            }

            // Verify all required components exists...
            foreach (Type requiredComponent in requiredComponents)
            {
                if (components.Has(requiredComponent) == false)
                {
                    throw new EntityTemplateException(key, $"Error creating {nameof(EntityTemplate)}, missing required component. Template = '{key.Name}', Component = '{requiredComponent.GetFormattedName()}'");
                }
            }

            group = EntityGroup.Create(key.Name, components.Keys);
            return new DynamicEntityDescriptor<VoidHuntersEntityDescriptor>(components);
        }

        private static void PopulateComponentCollections(
            Key<IEntityTemplate> key,
            IEntityTemplateFragmentService entityTemplateFragmentService,
            ref ComponentBuilderDictionary components,
            ref HashSet<Type> requiredComponents,
            ref Queue<Key<IEntityTemplate>> enqueuedTemplates,
            ref HashSet<Key<IEntityTemplate>> populatedTemplates)
        {
            if (populatedTemplates.Add(key) == false)
            {
                return;
            }

            foreach (EntityTemplateFragment fragment in entityTemplateFragmentService.GetByKey(key))
            {
                foreach (IEntityComponent component in fragment.Components)
                {
                    if (components.Has(component.GetType()) == false)
                    {
                        components.Set(component);
                    }
                }

                foreach (Type requiredComponent in fragment.RequiredComponents)
                {
                    requiredComponents.Add(requiredComponent);
                }

                if (fragment.Inherit is not null)
                {
                    enqueuedTemplates.Enqueue(fragment.Inherit.Value);
                }
            }
        }
    }
}

using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Common.Services;
using Guppy.Core.Logging.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities
{
    public class EntityTemplate : IEntityTemplate
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly ILogger _logger;
        private readonly ComponentSystemInvoker.ComponentSystemInvokerDelegateSequenceGroup<OnDespawnSequenceGroupEnum> _onDespawnSystemInvokers;
        private readonly ComponentSystemInvoker.ComponentSystemInvokerDelegateSequenceGroup<OnSpawnSequenceGroupEnum> _onSpawnSystemInvokers;
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
            IEntityFunctions functions,
            ILogger logger
        )
        {
            this._uniqueNumberProvider = uniqueNumberProvider;
            this._factory = factory;
            this._functions = functions;
            this._entitiesDB = null!;
            this._logger = logger;

            this._onDespawnSystemInvokers = new(false);
            this._onSpawnSystemInvokers = new(false);
            this._serializers = null!;

            this._descriptor = EntityTemplate.BuildDescriptor(key, entityTemplateService, out var components, out this._group);

            this.Key = key;
            this.Components = components;
        }

        public void Initialize(
            EntitiesDB entitiesDB,
            IScopedSystemService systemService,
            IComponentSerializerService componentSerializerService)
        {
            this._entitiesDB = entitiesDB;

            // Load serializers
            this._serializers = componentSerializerService.GetComponentSerializersByDescriptor(this._descriptor);

            // Generate despawn engine invokers
            // Responsible for calling IOnSpawnSystem & IOnDespawnSystem engines
            List<ComponentSystemInvoker> onDespawnSystemInvokers = [];
            List<ComponentSystemInvoker> onSpawnSystemInvokers = [];

            onDespawnSystemInvokers.AddRange(ComponentSystemInvoker.Create<OnDespawnSequenceGroupEnum>(typeof(OnDespawnSystemInvoker<>), typeof(IOnDespawnSystem<>), this.Components.Keys, systemService.GetAll(), this._entitiesDB, x => x.GetMethod("OnDespawn") ?? throw new NotImplementedException()).ToList());
            onDespawnSystemInvokers.AddRange(ComponentSystemInvoker.Create<OnDespawnSequenceGroupEnum>(typeof(OnDespawnSystemInvoker<,>), typeof(IOnDespawnSystem<,>), this.Components.Keys, systemService.GetAll(), this._entitiesDB, x => x.GetMethod("OnDespawn") ?? throw new NotImplementedException()).ToList());

            onSpawnSystemInvokers.AddRange(ComponentSystemInvoker.Create<OnSpawnSequenceGroupEnum>(typeof(OnSpawnSystemInvoker<>), typeof(IOnSpawnSystem<>), this.Components.Keys, systemService.GetAll(), this._entitiesDB, x => x.GetMethod("OnSpawn") ?? throw new NotImplementedException()).ToList());
            onSpawnSystemInvokers.AddRange(ComponentSystemInvoker.Create<OnSpawnSequenceGroupEnum>(typeof(OnSpawnSystemInvoker<,>), typeof(IOnSpawnSystem<,>), this.Components.Keys, systemService.GetAll(), this._entitiesDB, x => x.GetMethod("OnSpawn") ?? throw new NotImplementedException()).ToList());

            this._onDespawnSystemInvokers.Add(onDespawnSystemInvokers);
            this._onSpawnSystemInvokers.Add(onSpawnSystemInvokers);
        }

        public EntityInitializer HardSpawnEntity(in VhId sourceEventId, in EntityGlobalId globalId, out EntityLocalId localId)
        {
            // Create a new EGID for the entity
            EGID egid = new(this._uniqueNumberProvider.GetUInt32(), this._group.Value);
            localId = new(egid);

            this._logger.Verbose("HardSpawnInstanceEntity - GlobalId = {GlobalId}, LocalId = {localId}, Template = {Tempalte}", globalId, localId, this.Key.Name);

            // Invoke Svelto factory and initialize instance with common component values
            EntityInitializer initializer = this._factory.BuildEntity(egid, this._descriptor);
            initializer.Init(localId);
            initializer.Init(globalId);
            initializer.Init(new EntityStatus(EntityStatusEnum.HardSpawned));

            return initializer;
        }

        public void SoftSpawnEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status)
        {
            this._logger.Verbose("SoftSpawnInstanceEntity - GlobalId = {GlobalId}, LocalId = {localId}, Template = {Tempalte}", entity.GlobalId, entity.LocalId, this.Key.Name);
            this._onSpawnSystemInvokers.Invoke(sourceEventId, this, entity);
        }

        public void SoftDespawnEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status)
        {
            this._logger.Verbose("SoftDespawnInstanceEntity - GlobalId = {GlobalId}, LocalId = {localId}, Template = {Tempalte}", entity.GlobalId, entity.LocalId, this.Key.Name);
            this._onDespawnSystemInvokers.Invoke(sourceEventId, this, entity);
        }

        public void HardDespawnEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status)
        {
            this._logger.Verbose("HardDespawnInstanceEntity - GlobalId = {GlobalId}, LocalId = {localId}, Template = {Tempalte}", entity.GlobalId, entity.LocalId, this.Key.Name);
            this._functions.RemoveEntity<VoidHuntersEntityDescriptor>(entity.LocalId.Value);
        }

        public void SerializeEntity(ref EntityWriter writer, in Entity entity, in SerializationOptions options)
        {
            foreach (IComponentSerializer serializer in this._serializers)
            {
                serializer.Serialize(ref writer, in entity, this._entitiesDB, in options);
            }
        }

        public void DeserializeEntity(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            foreach (IComponentSerializer serializer in this._serializers)
            {
                serializer.Deserialize(in sourceId, in options, ref reader, in entity);
            }
        }

        public IEnumerable<Type> GetAllDistinctComponentTypes()
        {
            return this._descriptor.componentsToBuild.Select(x => x.GetEntityComponentType())
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
            EntityTemplateFlagsEnum entityTemplateFlags = EntityTemplateFlagsEnum.None;

            // Register default components...
            components.Set(new EntityLocalId());
            components.Set(new EntityGlobalId());
            components.Set(new EntityStatus());
            components.Set(new Common.Components.EntityTemplate(key));

            enqueuedFragments.Enqueue(key);
            while (enqueuedFragments.TryDequeue(out var enqueuedTemplate) == true)
            {
                EntityTemplate.PopulateComponentCollections(
                    enqueuedTemplate,
                    entityTemplateService,
                    ref components,
                    ref requiredComponents,
                    ref enqueuedFragments,
                    ref populatedTemplateKeys,
                    ref entityTemplateFlags);
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
            ref HashSet<Key<IEntityTemplate>> populatedTemplates,
            ref EntityTemplateFlagsEnum entityTemplateFlags)
        {
            if (populatedTemplates.Add(key) == false)
            {
                return;
            }

            foreach (EntityTemplateFragment fragment in entityTemplateFragmentService.GetByKey(key))
            {
                entityTemplateFlags |= fragment.Flags;

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
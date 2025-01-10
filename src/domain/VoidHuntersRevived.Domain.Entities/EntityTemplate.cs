using Guppy.Core.Common.Extensions.System;
using Serilog;
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

namespace VoidHuntersRevived.Domain.Entities
{
    internal sealed class EntityTemplate : IEntityTemplate
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly ILogger _logger;
        private readonly ComponentEngineInvoker.ComponentEngineInvokerDelegateSequenceGroup<OnDespawnSequenceGroupEnum> _onDespawnEngineInvokers;
        private readonly ComponentEngineInvoker.ComponentEngineInvokerDelegateSequenceGroup<OnSpawnSequenceGroupEnum> _onSpawnEngineInvokers;
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

            this._onDespawnEngineInvokers = new(false);
            this._onSpawnEngineInvokers = new(false);
            this._serializers = null!;

            this._descriptor = BuildDescriptor(key, entityTemplateService, out var components, out this._group);

            this.Key = key;
            this.Components = components;
        }

        public void Initialize(
            EntitiesDB entitiesDB,
            IEngineService engineService,
            IComponentSerializerService componentSerializerService)
        {
            this._entitiesDB = entitiesDB;

            // Load serializers
            this._serializers = componentSerializerService.GetComponentSerializersByDescriptor(this._descriptor);

            // Generate despawn engine invokers
            // Responsible for calling IOnSpawnEngine & IOnDespawnEngine engines
            List<ComponentEngineInvoker> onDespawnEngineInvokers = [];
            List<ComponentEngineInvoker> onSpawnEngineInvokers = [];

            onDespawnEngineInvokers.AddRange(ComponentEngineInvoker.Create<OnDespawnSequenceGroupEnum>(typeof(OnDespawnEngineInvoker<>), typeof(IOnDespawnEngine<>), this.Components.Keys, engineService, this._entitiesDB, x => x.GetMethod("OnDespawn") ?? throw new NotImplementedException()).ToList());
            onDespawnEngineInvokers.AddRange(ComponentEngineInvoker.Create<OnDespawnSequenceGroupEnum>(typeof(OnDespawnEngineInvoker<,>), typeof(IOnDespawnEngine<,>), this.Components.Keys, engineService, this._entitiesDB, x => x.GetMethod("OnDespawn") ?? throw new NotImplementedException()).ToList());

            onSpawnEngineInvokers.AddRange(ComponentEngineInvoker.Create<OnSpawnSequenceGroupEnum>(typeof(OnSpawnEngineInvoker<>), typeof(IOnSpawnEngine<>), this.Components.Keys, engineService, this._entitiesDB, x => x.GetMethod("OnSpawn") ?? throw new NotImplementedException()).ToList());
            onSpawnEngineInvokers.AddRange(ComponentEngineInvoker.Create<OnSpawnSequenceGroupEnum>(typeof(OnSpawnEngineInvoker<,>), typeof(IOnSpawnEngine<,>), this.Components.Keys, engineService, this._entitiesDB, x => x.GetMethod("OnSpawn") ?? throw new NotImplementedException()).ToList());

            this._onDespawnEngineInvokers.Add(onDespawnEngineInvokers);
            this._onSpawnEngineInvokers.Add(onSpawnEngineInvokers);
        }

        #region Instance Entity Methods
        public EntityInitializer HardSpawnInstanceEntity(in VhId sourceEventId, in EntityGlobalId globalId, out EntityLocalId localId)
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

        public void SoftSpawnInstanceEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status)
        {
            this._logger.Verbose("SoftSpawnInstanceEntity - GlobalId = {GlobalId}, LocalId = {localId}, Template = {Tempalte}", entity.GlobalId, entity.LocalId, this.Key.Name);
            this._onSpawnEngineInvokers.Invoke(sourceEventId, this, entity);
        }

        public void SoftDespawnInstanceEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status)
        {
            this._logger.Verbose("SoftDespawnInstanceEntity - GlobalId = {GlobalId}, LocalId = {localId}, Template = {Tempalte}", entity.GlobalId, entity.LocalId, this.Key.Name);
            this._onDespawnEngineInvokers.Invoke(sourceEventId, this, entity);
        }

        public void HardDespawnInstanceEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status)
        {
            this._logger.Verbose("HardDespawnInstanceEntity - GlobalId = {GlobalId}, LocalId = {localId}, Template = {Tempalte}", entity.GlobalId, entity.LocalId, this.Key.Name);
            this._functions.RemoveEntity<VoidHuntersEntityDescriptor>(entity.LocalId.Value);
        }

        public void SerializeInstanceEntity(ref EntityWriter writer, in Entity entity, in SerializationOptions options)
        {
            foreach (IComponentSerializer serializer in this._serializers)
            {
                serializer.Serialize(ref writer, in entity, this._entitiesDB, in options);
            }
        }

        public void DeserializeInstanceEntity(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            foreach (IComponentSerializer serializer in this._serializers)
            {
                serializer.Deserialize(in sourceId, in options, ref reader, in entity);
            }
        }
        #endregion

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

            // Register default components...
            components.Set(new EntityLocalId());
            components.Set(new EntityGlobalId());
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
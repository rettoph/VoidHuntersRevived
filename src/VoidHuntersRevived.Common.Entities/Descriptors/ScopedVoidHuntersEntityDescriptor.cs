using Autofac;
using Guppy.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Utilities;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public abstract class ScopedVoidHuntersEntityDescriptor
    {
        [ThreadStatic]
        internal static uint EntityId;

        private readonly FasterList<ComponentSerializer> _serializers;

        public readonly VoidHuntersEntityDescriptor GlobalDescriptor;

        public ScopedVoidHuntersEntityDescriptor(VoidHuntersEntityDescriptor globalDescriptor, ILifetimeScope scope)
        {
            this.GlobalDescriptor = globalDescriptor;

            _serializers = new FasterList<ComponentSerializer>(globalDescriptor.ComponentManagers.Count());
            foreach(ComponentManager manager in globalDescriptor.ComponentManagers)
            {
                _serializers.Add(manager.SerializerFactory.Create(scope));
            }
        }

        public void Serialize(EntityWriter writer, EGID egid, EntitiesDB entitiesDB, uint index)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Serialize(writer, index, egid.groupID, entitiesDB);
            }
        }

        public void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            foreach (ComponentSerializer serializer in _serializers)
            {
                serializer.Deserialize(reader, ref initializer, in id);
            }
        }

        public abstract EntityInitializer Spawn(IEntityFactory factory, VhId vhid, out EntityId id);
        public abstract void SoftDespawn(IEntityService entities, in EntityId id);
        public abstract void HardDespawn(IEntityService entities, IEntityFunctions functions, in EntityId id);

        public static ScopedVoidHuntersEntityDescriptor Build(VoidHuntersEntityDescriptor descriptor, ILifetimeScope scope)
        {
            Type spawnerType = typeof(ScopedEntityDescriptor<>).MakeGenericType(descriptor.GetType());

            return (ScopedVoidHuntersEntityDescriptor)Activator.CreateInstance(spawnerType, descriptor, scope)!;
        }
    }

    public class ScopedEntityDescriptor<TDescriptor> : ScopedVoidHuntersEntityDescriptor
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        public readonly ExclusiveGroup Group = new ExclusiveGroup();
        public new readonly TDescriptor GlobalDescriptor;

        private FasterList<OnDespawnEngineInvoker> _onDespawnEngineInvokers;

        public ScopedEntityDescriptor(TDescriptor globalDescriptor, ILifetimeScope scope) : base(globalDescriptor, scope)
        {
            this.GlobalDescriptor = globalDescriptor;

            var engines = scope.Resolve<IEngineService>().All();
            _onDespawnEngineInvokers = new FasterList<OnDespawnEngineInvoker>();
            foreach(Type componentType in this.GlobalDescriptor.ComponentManagers.Select(x => x.Type))
            {
                if(OnDespawnEngineInvoker.Create(componentType, engines, out var invoker))
                {
                    _onDespawnEngineInvokers.Add(invoker);
                }
            }    
        }

        public override EntityInitializer Spawn(IEntityFactory factory, VhId vhid, out EntityId id)
        {
            EGID egid = new EGID(EntityId++, Group);
            id = new EntityId(egid, vhid);

            EntityInitializer initializer = factory.BuildEntity(egid, this.GlobalDescriptor);
            initializer.Init(id);

            return initializer;
        }

        public override void SoftDespawn(IEntityService entities, in EntityId id)
        {
            ref EntityStatus status = ref entities.QueryById<EntityStatus>(id, out GroupIndex groupIndex);
            status.Status = EntityStatusType.SoftDespawned;

            for(int i=0; i<_onDespawnEngineInvokers.count; i++)
            {
                _onDespawnEngineInvokers[i].Invoke(entities, id, groupIndex);
            }
        }

        public override void HardDespawn(IEntityService entities, IEntityFunctions functions, in EntityId id)
        {
            ref EntityStatus status = ref entities.QueryById<EntityStatus>(id, out GroupIndex groupIndex);
            status.Status = EntityStatusType.HardDespawned;

            functions.RemoveEntity<TDescriptor>(id.EGID);
        }
    }
}

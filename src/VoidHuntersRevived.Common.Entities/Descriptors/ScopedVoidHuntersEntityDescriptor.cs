using Autofac;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;

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
        public abstract void Despawn(IEntityFunctions functions, in EGID egid);

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

        public ScopedEntityDescriptor(TDescriptor globalDescriptor, ILifetimeScope scope) : base(globalDescriptor, scope)
        {
            this.GlobalDescriptor = globalDescriptor;
        }

        public override EntityInitializer Spawn(IEntityFactory factory, VhId vhid, out EntityId id)
        {
            EGID egid = new EGID(EntityId++, Group);
            id = new EntityId(egid, vhid);

            EntityInitializer initializer = factory.BuildEntity(egid, this.GlobalDescriptor);
            initializer.Init(id);

            return initializer;
        }

        public override void Despawn(IEntityFunctions functions, in EGID egid)
        {
            functions.RemoveEntity<TDescriptor>(egid);
        }
    }
}

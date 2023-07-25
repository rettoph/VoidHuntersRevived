﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    internal abstract class VoidHuntersEntityDescriptorSpawner
    {
        [ThreadStatic]
        internal static uint EntityId;

        public readonly VoidHuntersEntityDescriptor Descriptor;

        protected VoidHuntersEntityDescriptorSpawner(VoidHuntersEntityDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public abstract EntityInitializer Spawn(IEntityFactory factory, VhId vhid, out EntityId id);
        public abstract void Despawn(IEntityFunctions functions, in EGID egid);

        public static VoidHuntersEntityDescriptorSpawner Build(VoidHuntersEntityDescriptor descriptor)
        {
            Type spawnerType = typeof(VoidHuntersEntityDescriptorSpawner<>).MakeGenericType(descriptor.GetType());

            return (VoidHuntersEntityDescriptorSpawner)Activator.CreateInstance(spawnerType, descriptor)!;
        }
    }

    internal sealed class VoidHuntersEntityDescriptorSpawner<TDescriptor> : VoidHuntersEntityDescriptorSpawner
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        [ThreadStatic]
        public readonly ExclusiveGroup Group = new ExclusiveGroup();
        public new readonly TDescriptor Descriptor;

        public VoidHuntersEntityDescriptorSpawner(TDescriptor descriptor) : base(descriptor)
        {
            Descriptor = descriptor;
        }

        public override EntityInitializer Spawn(IEntityFactory factory, VhId vhid, out EntityId id)
        {
            EGID egid = new EGID(EntityId++, Group);
            id = new EntityId(egid, vhid);

            EntityInitializer initializer = factory.BuildEntity(egid, this.Descriptor);
            initializer.Init(id);

            return initializer;
        }

        public override void Despawn(IEntityFunctions functions, in EGID egid)
        {
            functions.RemoveEntity<TDescriptor>(egid);
        }
    }
}

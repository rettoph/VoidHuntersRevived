﻿using Standart.Hash.xxHash;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Common.Entities
{
    public abstract class EntityType : IEntityType
    {
        private static List<EntityType> _list = new List<EntityType>();

        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly string Name;
        public readonly VhId Id;

        VoidHuntersEntityDescriptor IEntityType.Descriptor => this.Descriptor;
        string IEntityType.Name => this.Name;
        VhId IEntityType.Id => this.Id;

        internal unsafe EntityType(VhId nameSpace, string name, VoidHuntersEntityDescriptor descriptor)
        {
            this.Name = name;

            uint128 nameHash = xxHash128.ComputeHash(name);
            VhId* pNameHash = (VhId*)&nameHash;

            this.Id = nameSpace.Create(pNameHash[0]);

            _list.Add(this);
            this.Descriptor = descriptor;
        }

        public abstract IEntityTypeConfiguration BuildConfiguration();

        public abstract void DestroyEntity(IEntityFunctions functions, in EGID egid);
        public abstract EntityInitializer CreateEntity(IEntityFactory factory, VhId vhid);

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }
    }

    public sealed class EntityType<TDescriptor> : EntityType, IEntityType<TDescriptor>
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        private static uint EntityId;
        private static readonly ExclusiveGroup Group = new ExclusiveGroup();

        public readonly new TDescriptor Descriptor;

        TDescriptor IEntityType<TDescriptor>.Descriptor => this.Descriptor;

        public EntityType(VhId nameSpace, string name) : base(nameSpace, name, new TDescriptor())
        {
            this.Descriptor = new TDescriptor();
        }

        public override IEntityTypeConfiguration BuildConfiguration()
        {
            return new EntityTypeConfiguration<TDescriptor>(this);
        }

        public override EntityInitializer CreateEntity(IEntityFactory factory, VhId vhid)
        {
            EGID egid = new EGID(EntityId++, Group);
            EntityInitializer initializer = factory.BuildEntity(egid, this.Descriptor);
            initializer.Init(new EntityVhId() { Value = vhid });

            return initializer;
        }

        public override void DestroyEntity(IEntityFunctions functions, in EGID egid)
        {
            functions.RemoveEntity<TDescriptor>(egid);
        }
    }
}

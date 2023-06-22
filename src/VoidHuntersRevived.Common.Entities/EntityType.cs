using Standart.Hash.xxHash;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public abstract class EntityType
    {
        private static List<EntityType> _list = new List<EntityType>();

        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly string Name;
        public readonly VhId Id;

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
        public abstract EntityInitializer CreateEntity(IEntityFactory factory);

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }
    }

    public class EntityType<TDescriptor> : EntityType
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        private static uint EntityId;
        private static readonly ExclusiveGroup Group = new ExclusiveGroup();

        public readonly new TDescriptor Descriptor;

        public EntityType(VhId nameSpace, string name) : base(nameSpace, name, new TDescriptor())
        {
            this.Descriptor = new TDescriptor();
        }

        public override IEntityTypeConfiguration BuildConfiguration()
        {
            return new EntityTypeConfiguration<TDescriptor>(this);
        }

        public override EntityInitializer CreateEntity(IEntityFactory factory)
        {
            EGID egid = new EGID(EntityId++, Group);
            return factory.BuildEntity(egid, this.Descriptor);
        }

        public override void DestroyEntity(IEntityFunctions functions, in EGID egid)
        {
            functions.RemoveEntity<TDescriptor>(egid);
        }
    }
}

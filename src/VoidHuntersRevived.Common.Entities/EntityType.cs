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
    public abstract class EntityType : IEntityType
    {
        private static List<EntityType> _list = new List<EntityType>();

        public readonly EntityTypeId Id;
        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly string Key;

        EntityTypeId IEntityType.Id => this.Id;
        VoidHuntersEntityDescriptor IEntityType.Descriptor => this.Descriptor;
        string IEntityType.Key => this.Key;

        internal unsafe EntityType(VhId nameSpace, string key, VoidHuntersEntityDescriptor descriptor)
        {
            this.Key = key;

            uint128 nameHash = xxHash128.ComputeHash(key);
            VhId* pNameHash = (VhId*)&nameHash;

            this.Id = new EntityTypeId(nameSpace.Create(pNameHash[0]));

            _list.Add(this);
            this.Descriptor = descriptor;
        }

        public abstract IEntityTypeConfiguration BuildConfiguration();

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }
    }

    public sealed class EntityType<TDescriptor> : EntityType, IEntityType<TDescriptor>
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        public readonly new TDescriptor Descriptor;

        TDescriptor IEntityType<TDescriptor>.Descriptor => this.Descriptor;

        public EntityType(string key) : base(NameSpace<TDescriptor>.Instance, key, new TDescriptor())
        {
            this.Descriptor = new TDescriptor();
        }

        public override IEntityTypeConfiguration BuildConfiguration()
        {
            return new EntityTypeConfiguration<TDescriptor>(this);
        }
    }
}

using Standart.Hash.xxHash;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public abstract class EntityType
    {
        private static List<EntityType> _list = new List<EntityType>();

        public readonly string Name;
        public readonly VhId Id;

        internal unsafe EntityType(VhId nameSpace, string name)
        {
            this.Name = name;

            uint128 nameHash = xxHash128.ComputeHash(name);
            VhId* pNameHash = (VhId*)&nameHash;

            this.Id = nameSpace.Create(pNameHash[0]);

            _list.Add(this);
        }

        public abstract IEntityTypeConfiguration BuildConfiguration();

        public static IEnumerable<EntityType> All()
        {
            return _list;
        }
    }

    public class EntityType<TDescriptor> : EntityType
        where TDescriptor : IEntityDescriptor, new()
    {
        public static readonly ExclusiveGroup Group = new ExclusiveGroup();

        public EntityType(VhId nameSpace, string name) : base(nameSpace, name)
        {
        }

        public override IEntityTypeConfiguration BuildConfiguration()
        {
            return new EntityTypeConfiguration<TDescriptor>(this);
        }
    }
}

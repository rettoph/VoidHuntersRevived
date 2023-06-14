using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities
{
    internal sealed class EntityConfiguration : IEntityConfiguration
    {
        internal EntityTypeConfiguration typeConfiguration;

        public EntityName Name { get; }
        public EntityType Type { get; set; }

        public EntityConfiguration(EntityName name)
        {
            this.typeConfiguration = null!;

            this.Name = name;
            this.Type = null!;
        }

        internal void Initialize(EntityTypeService types)
        {
            this.typeConfiguration = types.GetConfiguration(this.Type);
        }

        public IEntityConfiguration SetType(EntityType type)
        {
            this.Type = type;

            return this;
        }

        public IEntityConfiguration AddProperty<T>(T property)
            where T : class, IEntityProperty
        {
            throw new NotImplementedException();
        }
    }
}

using Svelto.DataStructures;
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
        private readonly List<PreCacheProperty> _properties;

        internal EntityTypeConfiguration typeConfiguration;
        internal readonly FasterList<PropertyCache> properties;

        public EntityName Name { get; }
        public EntityType Type { get; set; }

        public EntityConfiguration(EntityName name)
        {
            _properties = new List<PreCacheProperty>();

            this.typeConfiguration = null!;
            this.properties = new FasterList<PropertyCache>();

            this.Name = name;
            this.Type = null!;
        }

        internal void Initialize(EntityTypeService types, EntityPropertyService properties)
        {
            this.typeConfiguration = types.GetConfiguration(this.Type);

            foreach(PreCacheProperty preProperty in _properties)
            {
                this.properties.Add(preProperty.Iniitalize(properties));
            }
        }

        public IEntityConfiguration SetType(EntityType type)
        {
            this.Type = type;

            return this;
        }

        public IEntityConfiguration AddProperty<T>(T property)
            where T : class, IEntityProperty
        {
            _properties.Add(new PreCacheProperty<T>(property));

            return this;
        }
    }
}

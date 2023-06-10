using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Domain.ECS.Abstractions;
using VoidHuntersRevived.Domain.ECS.Services;

namespace VoidHuntersRevived.Domain.ECS
{
    internal sealed class EntityTypeConfiguration : IEntityTypeConfiguration
    {
        private readonly HashSet<EntityType> _baseTypes;
        private readonly List<IComponentBuilder> _builders;

        public EntityType Type { get; }

        public EntityTypeConfiguration(EntityType type)
        {
            _baseTypes = new HashSet<EntityType>();
            _builders = new List<IComponentBuilder>();

            this.Type = type;
        }

        public IEntityTypeConfiguration Inherits(EntityType baseType)
        {
            _baseTypes.Add(baseType);

            return this;
        }

        public IEntityTypeConfiguration Has<T>()
            where T : unmanaged
        {
            _builders.Add(Component<T>.Builder);

            return this;
        }

        internal EntityDescriptorGroup BuildEntityDescriptorGroup(EntityTypeService entityTypes)
        {
            HashSet<EntityType> types = new HashSet<EntityType>();
            HashSet<IComponentBuilder> components = new HashSet<IComponentBuilder>();

            this.GetComponentBuildersRecersive(entityTypes, types, components);

            return new EntityDescriptorGroup(
                descriptor: new DynamicEntityDescriptor<EntityDescriptor>(components.ToArray()),
                group: new ExclusiveGroup());
        }

        private void GetComponentBuildersRecersive(
            EntityTypeService entityTypes,
            HashSet<EntityType> types, 
            HashSet<IComponentBuilder> components)
        {
            if(!types.Add(this.Type))
            {
                return;
            }

            foreach (IComponentBuilder builder in _builders)
            {
                components.Add(builder);
            }

            foreach (EntityType type in _baseTypes)
            {
                entityTypes.GetConfiguration(type).GetComponentBuildersRecersive(entityTypes, types, components);
            }
        }
    }
}

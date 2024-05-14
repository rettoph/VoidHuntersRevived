using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    [AutoLoad]
    internal class HasManyEntityInitializer : BaseEntityInitializer, IDisposable
    {
        private readonly StaticValue<HasManyEntityInitializer, IEntityService> _entities;

        public HasManyEntityInitializer(IEntityService entities)
        {
            _entities = new StaticValue<HasManyEntityInitializer, IEntityService>(entities);

            this.WithInstanceInitializer(type => type.Descriptor.componentsToBuild.Any(x =>
            {
                Type componentType = x.GetEntityComponentType();

                if (componentType.IsConstructedGenericType == false)
                {
                    return false;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(HasMany<>))
                {
                    return false;
                }

                return true;

            }), this.InitializeInstanceParentComponents);

            this.WithTypeInitializer(type => type.Descriptor.StaticDescriptor.componentsToBuild.Any(x =>
            {
                Type componentType = x.GetEntityComponentType();

                if (componentType.IsConstructedGenericType == false)
                {
                    return false;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(HasMany<>))
                {
                    return false;
                }

                return true;

            }), this.InitializeTypeParentComponents);
        }

        public void Dispose()
        {
            _entities.Dispose();
        }

        private void InitializeInstanceParentComponents(IEntityService entities, IEntityType type, ref EntityInitializer initializer, in EntityId id)
        {
            throw new NotImplementedException();
        }

        private void InitializeTypeParentComponents(IEntityService entities, IEntityType type, ref EntityInitializer initializer, in EntityId id)
        {
            // throw new NotImplementedException();
        }


    }
}

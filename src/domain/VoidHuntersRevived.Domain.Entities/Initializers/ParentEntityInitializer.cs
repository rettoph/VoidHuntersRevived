using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    [AutoLoad]
    internal class ParentEntityInitializer : BaseEntityInitializer
    {
        public ParentEntityInitializer()
        {
            this.WithInstanceInitializer(type => type.Descriptor.componentsToBuild.Any(x =>
            {
                Type componentType = x.GetEntityComponentType();

                if (componentType.IsConstructedGenericType == false)
                {
                    return false;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(Parent<>))
                {
                    return false;
                }

                return true;

            }), this.InitializeInstanceParentComponents);

            this.WithStaticInitializer(type => type.Descriptor.StaticDescriptor.componentsToBuild.Any(x =>
            {
                Type componentType = x.GetEntityComponentType();

                if (componentType.IsConstructedGenericType == false)
                {
                    return false;
                }

                if (componentType.GetGenericTypeDefinition() != typeof(Parent<>))
                {
                    return false;
                }

                return true;

            }), this.InitializeStaticParentComponents);
        }

        private void InitializeInstanceParentComponents(IEntityType type, ref EntityInitializer initializer, in EntityId id)
        {
            throw new NotImplementedException();
        }

        private void InitializeStaticParentComponents(IEntityType type, ref EntityInitializer initializer)
        {
            throw new NotImplementedException();
        }
    }
}

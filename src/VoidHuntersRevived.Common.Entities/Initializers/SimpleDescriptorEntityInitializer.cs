using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Initializers
{
    public abstract class SimpleDescriptorEntityInitializer<TDescriptor> : IEntityInitializer
        where TDescriptor : VoidHuntersEntityDescriptor
    {
        public IEntityType[] RegisterTypes { get; }

        protected SimpleDescriptorEntityInitializer()
        {
            this.RegisterTypes = Array.Empty<IEntityType>();
        }

        public bool ShouldInitialize(IEntityType entityType)
        {
            return entityType.Descriptor.GetType().IsAssignableTo<TDescriptor>();
        }

        public virtual InstanceEntityInitializerDelegate? InstanceInitializer(IEntityType entityType)
        {
            return null;
        }

        public virtual DisposeEntityInitializerDelegate? InstanceDisposer(IEntityType entityType)
        {
            return null;
        }

        public virtual StaticEntityInitializerDelegate? StaticInitializer(IEntityType entityType)
        {
            return null;
        }

        public virtual DisposeEntityInitializerDelegate? StaticDisposer(IEntityType entityType)
        {
            return null;
        }
    }
}

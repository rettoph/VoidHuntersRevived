using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Initializers
{
    internal abstract class SimpleTypeEntityInitializer : IEntityInitializer
    {
        public IEntityType[] ExplicitEntityTypes { get; }

        protected SimpleTypeEntityInitializer(IEntityType[] types)
        {
            this.ExplicitEntityTypes = types;

        }

        public bool ShouldInitialize(IEntityType entityType)
        {
            return this.ExplicitEntityTypes.Contains(entityType);
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

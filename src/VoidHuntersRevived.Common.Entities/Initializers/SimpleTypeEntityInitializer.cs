using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Initializers
{
    public abstract class SimpleTypeEntityInitializer : IEntityInitializer
    {
        public IEntityType[] RegisterTypes { get; }

        protected SimpleTypeEntityInitializer(IEntityType[] types)
        {
            this.RegisterTypes = types;

        }

        public bool ShouldInitialize(IEntityType entityType)
        {
            return this.RegisterTypes.Contains(entityType);
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

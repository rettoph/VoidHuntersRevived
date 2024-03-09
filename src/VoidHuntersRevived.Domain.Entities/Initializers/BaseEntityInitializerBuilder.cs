using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    internal class BaseEntityInitializerBuilder : IEntityInitializerBuilder
    {
        public InstanceEntityInitializerDelegate? InstanceInitializer;
        public StaticEntityInitializerDelegate? StaticInitializer;

        public DisposeEntityInitializerDelegate? InstanceDisposer;
        public DisposeEntityInitializerDelegate? StaticDisposer;

        public IEntityInitializerBuilder InitializeInstance(InstanceEntityInitializerDelegate initializer)
        {
            this.InstanceInitializer += initializer;

            return this;
        }

        public IEntityInitializerBuilder DisposeInstance(DisposeEntityInitializerDelegate disposer)
        {
            this.InstanceDisposer += disposer;

            return this;
        }

        public IEntityInitializerBuilder InitializeStatic(StaticEntityInitializerDelegate initializer)
        {
            this.StaticInitializer += initializer;

            return this;
        }

        public IEntityInitializerBuilder DisposeStatic(DisposeEntityInitializerDelegate disposer)
        {
            this.StaticDisposer += disposer;

            return this;
        }
    }
}

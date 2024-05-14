using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    internal sealed class EntityTypeInitializerWrapper : IEntityTypeInitializer
    {
        public InstanceEntityInitializerDelegate InstanceEntityInitializer;
        public StaticEntityInitializerDelegate StaticEntityInitializer;

        public DisposeEntityInitializerDelegate InstanceEntityDisposer;
        public DisposeEntityInitializerDelegate StaticEntityDisposer;

        public IEntityType Type { get; }

        public EntityTypeInitializerWrapper(IEntityType type, IEnumerable<IEntityInitializer> initializers)
        {
            this.Type = type;

            if (this.Type.InstanceComponents.Count > 0)
            {
                this.InstanceEntityInitializer = EntityInitializerHelper.BuildInstanceEntityInitializerDelegate(this.Type.InstanceComponents.Values) ?? throw new Exception();
            }

            if (this.Type.StaticComponents.Count > 0)
            {
                this.StaticEntityInitializer = EntityInitializerHelper.BuildStaticEntityInitializerDelegate(this.Type.StaticComponents.Values) ?? throw new Exception();
            }

            foreach (IEntityInitializer initializer in initializers.OrderBy(x => x.Order))
            {
                InstanceEntityInitializerDelegate? initializerInstanceInitializer = initializer.InstanceInitializer(this.Type);
                if (initializerInstanceInitializer is not null)
                {
                    InstanceEntityInitializer += initializerInstanceInitializer;
                }

                StaticEntityInitializerDelegate? initializerStaticInitializer = initializer.StaticInitializer(this.Type);
                if (initializerStaticInitializer is not null)
                {
                    StaticEntityInitializer += initializerStaticInitializer;
                }
            }

            foreach (IEntityInitializer initializer in initializers.OrderByDescending(x => x.Order))
            {
                DisposeEntityInitializerDelegate? initializerInstanceDisposer = initializer.InstanceDisposer(this.Type);
                if (initializerInstanceDisposer is not null)
                {
                    InstanceEntityDisposer += initializerInstanceDisposer;
                }

                DisposeEntityInitializerDelegate? initializerStaticDisposer = initializer.StaticDisposer(this.Type);
                if (initializerStaticDisposer is not null)
                {
                    StaticEntityDisposer += initializerStaticDisposer;
                }
            }

            if (InstanceEntityInitializer is null)
            {
                InstanceEntityInitializer = EntityTypeInitializerWrapper.DefaultInstanceInitializer;
            }

            if (InstanceEntityDisposer is null)
            {
                InstanceEntityDisposer = EntityTypeInitializerWrapper.DefaultDisposer;
            }

            if (StaticEntityInitializer is null)
            {
                StaticEntityInitializer = EntityTypeInitializerWrapper.DefaultStaticInitializer;
            }

            if (StaticEntityDisposer is null)
            {
                StaticEntityDisposer = EntityTypeInitializerWrapper.DefaultDisposer;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void InitializeInstance(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<Id<IEntityType>>(this.Type.Id);
            InstanceEntityInitializer(this.Type, ref initializer, in id);
        }

        public void InitializeStatic(ref EntityInitializer initializer)
        {
            StaticEntityInitializer(this.Type, ref initializer);
        }

        private static void DefaultInstanceInitializer(IEntityType type, ref EntityInitializer initializer, in EntityId id)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultStaticInitializer(IEntityType type, ref EntityInitializer initializer)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultDisposer(IEntityType type)
        {
            // throw new NotImplementedException();
        }
    }
}

using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    internal sealed class EntityTypeInitializerWrapper : IEntityTypeInitializer
    {
        public EntityInitializerDelegate InstanceEntityInitializer;
        public EntityInitializerDelegate TypeEntityInitializer;

        public DisposeEntityInitializerDelegate InstanceEntityDisposer;
        public DisposeEntityInitializerDelegate TypeEntityDisposer;

        public IEntityType Type { get; }

        public EntityTypeInitializerWrapper(IEntityType type, IEnumerable<IEntityInitializer> initializers)
        {
            this.Type = type;

            if (this.Type.InstanceComponents.Count > 0)
            {
                this.InstanceEntityInitializer = EntityInitializerHelper.BuildEntityInitializerDelegate(this.Type.InstanceComponents.Values) ?? throw new Exception();
            }

            if (this.Type.Components.Count > 0)
            {
                this.TypeEntityInitializer = EntityInitializerHelper.BuildEntityInitializerDelegate(this.Type.Components.Values) ?? throw new Exception();
            }

            foreach (IEntityInitializer initializer in initializers.OrderBy(x => x.Order))
            {
                EntityInitializerDelegate? initializerInstanceInitializer = initializer.InstanceInitializer(this.Type);
                if (initializerInstanceInitializer is not null)
                {
                    InstanceEntityInitializer += initializerInstanceInitializer;
                }

                EntityInitializerDelegate? initializerStaticInitializer = initializer.TypeInitializer(this.Type);
                if (initializerStaticInitializer is not null)
                {
                    TypeEntityInitializer += initializerStaticInitializer;
                }
            }

            foreach (IEntityInitializer initializer in initializers.OrderByDescending(x => x.Order))
            {
                DisposeEntityInitializerDelegate? initializerInstanceDisposer = initializer.InstanceDisposer(this.Type);
                if (initializerInstanceDisposer is not null)
                {
                    InstanceEntityDisposer += initializerInstanceDisposer;
                }

                DisposeEntityInitializerDelegate? initializerStaticDisposer = initializer.TypeDisposer(this.Type);
                if (initializerStaticDisposer is not null)
                {
                    TypeEntityDisposer += initializerStaticDisposer;
                }
            }

            if (InstanceEntityInitializer is null)
            {
                InstanceEntityInitializer = EntityTypeInitializerWrapper.DefaultInitializer;
            }

            if (InstanceEntityDisposer is null)
            {
                InstanceEntityDisposer = EntityTypeInitializerWrapper.DefaultDisposer;
            }

            if (TypeEntityInitializer is null)
            {
                TypeEntityInitializer = EntityTypeInitializerWrapper.DefaultInitializer;
            }

            if (TypeEntityDisposer is null)
            {
                TypeEntityDisposer = EntityTypeInitializerWrapper.DefaultDisposer;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void InitializeInstance(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer)
        {
            initializer.Init<Id<IEntityType>>(this.Type.Id);
            InstanceEntityInitializer(entities, this.Type, in id, ref initializer);
        }

        public void InitializeType(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer)
        {
            TypeEntityInitializer(entities, this.Type, in id, ref initializer);
        }

        private static void DefaultInitializer(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultDisposer(IEntityType type)
        {
            // throw new NotImplementedException();
        }
    }
}

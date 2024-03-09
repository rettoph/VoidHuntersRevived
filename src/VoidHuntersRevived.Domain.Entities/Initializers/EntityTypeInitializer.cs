using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    internal sealed class EntityTypeInitializer : IEntityTypeInitializer
    {
        public event InstanceEntityInitializerDelegate InstanceEntityInitializer;
        public event StaticEntityInitializerDelegate StaticEntityInitializer;

        public event DisposeEntityInitializerDelegate InstanceEntityDisposer;
        public event DisposeEntityInitializerDelegate StaticEntityDisposer;

        public IEntityType Type { get; }

        public EntityTypeInitializer(IEntityType type, IEnumerable<BaseEntityInitializerBuilder> partialInitializers)
        {
            this.Type = type;

            foreach (BaseEntityInitializerBuilder partialInitializer in partialInitializers)
            {
                if (partialInitializer.InstanceInitializer is not null)
                {
                    InstanceEntityInitializer += partialInitializer.InstanceInitializer;
                }

                if (partialInitializer.InstanceDisposer is not null)
                {
                    InstanceEntityDisposer += partialInitializer.InstanceDisposer;
                }

                if (partialInitializer.StaticInitializer is not null)
                {
                    StaticEntityInitializer += partialInitializer.StaticInitializer;
                }

                if (partialInitializer.StaticDisposer is not null)
                {
                    StaticEntityDisposer += partialInitializer.StaticDisposer;
                }
            }

            if (InstanceEntityInitializer is null)
            {
                InstanceEntityInitializer = EntityTypeInitializer.DefaultInstanceInitializer;
            }

            if (InstanceEntityDisposer is null)
            {
                InstanceEntityDisposer = EntityTypeInitializer.DefaultDisposer;
            }

            if (StaticEntityInitializer is null)
            {
                StaticEntityInitializer = EntityTypeInitializer.DefaultStaticInitializer;
            }

            if (StaticEntityDisposer is null)
            {
                StaticEntityDisposer = EntityTypeInitializer.DefaultDisposer;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void InitializeInstance(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<Id<IEntityType>>(this.Type.Id);
            InstanceEntityInitializer(entities, ref initializer, in id);

            this.Type.Descriptor.PostInitialize(entities, ref initializer, in id);
        }

        public void InitializeStatic(ref EntityInitializer initializer)
        {
            StaticEntityInitializer(ref initializer);
        }

        private static void DefaultInstanceInitializer(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultStaticInitializer(ref EntityInitializer initializer)
        {
            // throw new NotImplementedException();
        }

        private static void DefaultDisposer()
        {
            // throw new NotImplementedException();
        }
    }
}

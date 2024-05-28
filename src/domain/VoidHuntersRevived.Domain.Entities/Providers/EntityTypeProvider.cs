using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Providers
{
    internal sealed class EntityTypeProvider : IEntityTypeProvider
    {
        // Despite being nullable these delegates will have a default value defined
        // within the constructor. No need to check
        public EntityInitializerDelegate? InstanceEntityInitializer;
        public EntityInitializerDelegate? TypeEntityInitializer;

        public DisposeEntityInitializerDelegate? InstanceEntityDisposer;
        public DisposeEntityInitializerDelegate? TypeEntityDisposer;

        public IEntityType Type { get; }

        public EntityTypeProvider(IEntityType type, IEnumerable<IEntityInitializer> initializers)
        {
            this.Type = type;

            // Append hard coded component initialization delegates
            this.InstanceEntityInitializer += EntityInitializerHelper.BuildEntityInitializerDelegate(this.Type.InstanceComponents.Values);
            this.TypeEntityInitializer += EntityInitializerHelper.BuildEntityInitializerDelegate(this.Type.Components.Values);

            // Add relevent front-to-back delegates
            foreach (IEntityInitializer initializer in initializers.OrderBy(x => x.Order))
            {
                this.InstanceEntityInitializer += initializer.InstanceInitializer(Type);
                this.TypeEntityInitializer += initializer.TypeInitializer(Type);
            }

            // Add relevent back-to-front delegates
            foreach (IEntityInitializer initializer in initializers.OrderByDescending(x => x.Order))
            {
                this.InstanceEntityDisposer += initializer.InstanceDisposer(Type);
                this.TypeEntityDisposer += initializer.TypeDisposer(Type);
            }

            // Add some default values if no delegates were defined.
            this.InstanceEntityInitializer ??= EntityTypeProvider.DefaultInitializer;
            this.InstanceEntityDisposer ??= EntityTypeProvider.DefaultDisposer;
            this.TypeEntityInitializer ??= EntityTypeProvider.DefaultInitializer;
            this.TypeEntityDisposer ??= EntityTypeProvider.DefaultDisposer;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void InitializeInstance(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer)
        {
            initializer.Init(Type.Id);
            InstanceEntityInitializer!(entities, Type, in id, ref initializer);
        }

        public void InitializeType(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer)
        {
            TypeEntityInitializer!(entities, Type, in id, ref initializer);
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

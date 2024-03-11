using Guppy.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common.Initializers
{
    [Service<IEntityInitializer>(ServiceLifetime.Scoped, true)]
    public interface IEntityInitializer
    {
        /// <summary>
        /// Initializers are built in ascending order for initialization,
        /// descendin for disposing.
        /// 
        /// I dont love the use of a magic number, especially
        /// because a single initializer may define many delegates.
        /// 
        /// This may somehow change in the future.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Provides explicit <see cref="IEntityType"/> instances to be registered at runtime.
        /// If a <see cref="IEntityType"/> is not registered then an initializer for it will
        /// never be built. It is important that entities to be created in game are, in some fasion,
        /// defined by an <see cref="IEntityInitializer"/>.
        /// </summary>
        IEntityType[] ExplicitEntityTypes { get; }

        /// <summary>
        /// Indicates wether or not the current <see cref="IEntityInitializer"/> should be used when
        /// constructing a <see cref="IEntityTypeInitializer"/> instance for the given <paramref name="entityType"/>
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        bool ShouldInitialize(IEntityType entityType);

        InstanceEntityInitializerDelegate? InstanceInitializer(IEntityType entityType);
        DisposeEntityInitializerDelegate? InstanceDisposer(IEntityType entityType);
        StaticEntityInitializerDelegate? StaticInitializer(IEntityType entityType);
        DisposeEntityInitializerDelegate? StaticDisposer(IEntityType entityType);
    }
}

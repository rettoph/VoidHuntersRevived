using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Initializers
{
    [Service<IEntityTypeProviderInitializer>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public interface IEntityTypeProviderInitializer
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
        /// Run one time initialization on a given entity type.
        /// This provides time for runtime modifications to the backing
        /// type and descriptor if needed.
        /// </summary>
        /// <param name="entityTypeProvider"></param>
        void InitializeEntityTypeProvider(IEntityTypeProvider entityTypeProvider);

        EntityInitializerDelegate? GetEntityInitializer(IEntityTypeProvider entityTypeProvider);
        DisposeEntityInitializerDelegate? GetEntityDisposer(IEntityTypeProvider entityTypeProvider);
    }
}

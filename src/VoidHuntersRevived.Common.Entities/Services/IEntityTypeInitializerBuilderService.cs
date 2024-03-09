using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Initializers;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityTypeInitializerBuilderService
    {
        void Register(params IEntityType[] types);

        void Configure<TDescriptor>(Action<IEntityInitializerBuilder> builder)
            where TDescriptor : VoidHuntersEntityDescriptor;

        void Configure(IEntityType type, Action<IEntityInitializerBuilder> builder);

        Dictionary<IEntityType, IEntityTypeInitializer> Build();
    }
}

using Guppy.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Loaders
{
    [Service<ITeamLoader>(ServiceLifetime.Singleton, true)]
    public interface ITeamLoader
    {
        void Configure(ITeamService teams);
    }
}

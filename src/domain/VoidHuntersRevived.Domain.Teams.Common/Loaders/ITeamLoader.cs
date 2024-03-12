using Guppy.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Common.Loaders
{
    [Service<ITeamLoader>(ServiceLifetime.Singleton, true)]
    public interface ITeamLoader
    {
        void Configure(ITeamService teams);
    }
}

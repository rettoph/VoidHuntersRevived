using Guppy.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Loaders
{
    [Service<ITeamLoader>(ServiceLifetime.Singleton, true)]
    public interface ITeamLoader
    {
        void Configure(ITeamService teams);
    }
}

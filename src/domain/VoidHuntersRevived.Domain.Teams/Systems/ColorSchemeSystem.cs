using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Systems;

namespace VoidHuntersRevived.Domain.Teams.Systems
{
    public class ColorSchemeSystem(
        IEntityQueryService entityQueryService
    ) : BaseInheritTeamComponentSystem<ColorScheme>(entityQueryService)
    {
    }
}
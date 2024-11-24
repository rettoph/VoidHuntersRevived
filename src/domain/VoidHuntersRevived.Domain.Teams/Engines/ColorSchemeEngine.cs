using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Engines;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    internal class ColorSchemeEngine(IEntityQueryService entityQueryService) : BaseInheritTeamComponentEngine<ColorScheme>(entityQueryService)
    {
    }
}

using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Engines;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    internal class ColorSchemeEngine : BaseTeamInstanceComponentEngine<ColorScheme>
    {
        public ColorSchemeEngine(IEntityQueryService entityQueryService) : base(entityQueryService)
        {
        }
    }
}

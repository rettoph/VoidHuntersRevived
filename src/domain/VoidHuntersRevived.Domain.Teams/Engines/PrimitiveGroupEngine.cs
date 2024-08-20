using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Engines;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    internal class PrimitiveGroupEngine : BaseTeamInstanceComponentEngine<PrimitiveGroup>
    {
        public PrimitiveGroupEngine(IEntityQueryService entityQueryService) : base(entityQueryService)
        {
        }
    }
}

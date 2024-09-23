using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Engines;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    internal class PrimitiveGroupEngine : BaseTeamInstanceComponentEngine<PrimitiveGroup>
    {
        public PrimitiveGroupEngine(IEntityQueryService entityQueryService) : base(entityQueryService)
        {
        }
    }
}

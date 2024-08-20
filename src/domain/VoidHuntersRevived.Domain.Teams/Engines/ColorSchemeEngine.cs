using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Engines;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    /// <summary>
    /// Responsible for setting a new instance entity's color scheme.
    /// It will first use the teams color scheme, if any.
    /// Otherwise it will default to the entity type color scheme.
    /// 
    ///   1. Team component value
    ///   2. Instance entity's Type component value
    ///   3. Reset component value
    /// 
    /// </summary>
    [AutoLoad]
    internal class ColorSchemeEngine : BaseTeamInstanceComponentEngine<ColorScheme>
    {
        public ColorSchemeEngine(IEntityQueryService entityQueryService) : base(entityQueryService)
        {
        }
    }
}

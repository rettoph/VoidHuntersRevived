using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    public class InitializeEntityServicesSystem(EntityTemplateService entityTemplateService) : ISceneSystem<IStrategy>
    {
        private readonly EntityTemplateService _entityTemplateService = entityTemplateService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize(IStrategy strategy)
        {
            // this._entityTemplateService.TestInitialize(strategy);
        }
    }
}

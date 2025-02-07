using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    public class InitializeEntityServicesSystem(
        EntityTemplateService entityTemplateService
    ) : ISceneSystem,
        IInitializeSystem
    {
        private readonly EntityTemplateService _entityTemplateService = entityTemplateService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize()
        {
            // this._entityTemplateService.TestInitialize(strategy);
        }
    }
}

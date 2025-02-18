using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    public class InitializeEntityServicesSystem(
        ComponentSerializerService componentSerializerService,
        EntityTemplateService entityTemplateService
    ) : ISceneSystem,
        IInitializeSystem
    {
        private readonly ComponentSerializerService _componentSerializerService = componentSerializerService;
        private readonly EntityTemplateService _entityTemplateService = entityTemplateService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.PreInitialize)]
        public void Initialize()
        {
            this._componentSerializerService.Initialize();
            this._entityTemplateService.Initialize();
        }
    }
}

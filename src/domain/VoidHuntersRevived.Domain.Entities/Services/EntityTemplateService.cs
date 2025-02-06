using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTemplateService : StrategySystem, IEntityTemplateService, IOnInitializeSystem
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityTemplateFragmentService _entityTemplateFragmentService;
        private readonly Lazy<IComponentSerializerService> _componentSerializerService;
        private readonly EntitiesDB _entitiesDb;

        private readonly Dictionary<Key<IEntityTemplate>, IEntityTemplate> _templates;

        public EntityTemplateService(
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityTemplateFragmentService entityTemplateFragmentService,
            ILoggerService loggerService,
            Lazy<IComponentSerializerService> componentSerializerService,
            EnginesRoot enginesRoot,
            EntitiesDB entitiesDb)
        {
            this._uniqueNumberProvider = uniqueNumberProvider;
            this._entityTemplateFragmentService = entityTemplateFragmentService;
            this._componentSerializerService = componentSerializerService;
            this._entitiesDb = entitiesDb;

            // Create EntityTemplateProviders for all registered EntityTemplate instances
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();
            IEntityFunctions functions = enginesRoot.GenerateEntityFunctions();
            this._templates = this._entityTemplateFragmentService.GetAll()
                .Where(kvp => kvp.Value.Select(f => f.Flags).Aggregate((f1, f2) => f1 | f2).HasFlag(EntityTemplateFlagsEnum.Partial) == false)
                .ToDictionary(
                    keySelector: kvp => kvp.Key,
                    elementSelector: kvp =>
                    {
                        return (IEntityTemplate)new EntityTemplate(
                            kvp.Key,
                            this._entityTemplateFragmentService,
                            this._uniqueNumberProvider,
                            factory,
                            functions,
                            loggerService.GetLogger<EntityTemplate>()
                        );
                    });
        }

        [SequenceGroup<OnInitializeSequenceGroupEnum>(OnInitializeSequenceGroupEnum.PreInitialize)]
        public void OnInitialize(IStrategy strategy)
        {
            foreach (IEntityTemplate entityTemplateProvider in this._templates.Values)
            {
                entityTemplateProvider.Initialize(
                    entitiesDB: this._entitiesDb,
                    engineService: this.Strategy.Engines,
                    componentSerializerService: this._componentSerializerService.Value);
            }
        }

        public IEntityTemplate GetByKey(Key<IEntityTemplate> key)
        {
            return this._templates[key];
        }

        public IEnumerable<IEntityTemplate> WithComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent
        {
            return this._templates.Values.Where(x => x.Components.Has<TComponent>());
        }
    }
}
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Providers;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTemplateService : StrategyEngine, IEntityTemplateService, IQueryingEntitiesEngine, IOnInitializeEngine
    {
        private readonly IUniqueNumberProvider _uniqueNumberProvider;
        private readonly IEntityTemplateFragmentService _entityTemplateFragmentService;
        private readonly Lazy<IComponentSerializerService> _componentSerializerService;

        private readonly Dictionary<Key<IEntityTemplate>, IEntityTemplate> _templates;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityTemplateService(
            IUniqueNumberProvider uniqueNumberProvider,
            IEntityTemplateFragmentService entityTemplateFragmentService,
            ILoggerService loggerService,
            Lazy<IComponentSerializerService> componentSerializerService,
            EnginesRoot enginesRoot)
        {
            _uniqueNumberProvider = uniqueNumberProvider;
            _entityTemplateFragmentService = entityTemplateFragmentService;
            _componentSerializerService = componentSerializerService;

            // Create EntityTemplateProviders for all registered EntityTemplate instances
            IEntityFactory factory = enginesRoot.GenerateEntityFactory();
            IEntityFunctions functions = enginesRoot.GenerateEntityFunctions();
            _templates = _entityTemplateFragmentService.GetAll()
                .Where(kvp => kvp.Value.Select(f => f.Flags).Aggregate((f1, f2) => f1 | f2).HasFlag(EntityTemplateFlags.Partial) == false)
                .ToDictionary(
                    keySelector: kvp => kvp.Key,
                    elementSelector: kvp =>
                    {
                        return (IEntityTemplate)new EntityTemplate(
                            kvp.Key,
                            _entityTemplateFragmentService,
                            _uniqueNumberProvider,
                            factory,
                            functions,
                            loggerService.GetOrCreate<EntityTemplate>()
                        );
                    });
        }

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.PreInitialize)]
        public void OnInitialize(IStrategy strategy)
        {
            foreach (IEntityTemplate entityTemplateProvider in _templates.Values)
            {
                entityTemplateProvider.Initialize(
                    entitiesDB: this.entitiesDB,
                    engineService: this.Strategy.Engines,
                    componentSerializerService: _componentSerializerService.Value);
            }
        }

        public IEntityTemplate GetByKey(Key<IEntityTemplate> key)
        {
            return _templates[key];
        }

        public IEnumerable<IEntityTemplate> WithComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent
        {
            return _templates.Values.Where(x => x.Components.Has<TComponent>());
        }
    }
}

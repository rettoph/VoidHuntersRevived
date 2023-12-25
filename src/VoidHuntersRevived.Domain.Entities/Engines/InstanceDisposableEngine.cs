using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    internal sealed class InstanceDisposableEngine<T> : IEngine, IReactOnRemoveEx<T>, IQueryingEntitiesEngine
        where T : unmanaged, IEntityComponent, IDisposable
    {
        private static readonly string _tName = typeof(T).GetFormattedName();

        private readonly ILogger _logger;

        public InstanceDisposableEngine(ILogger logger)
        {
            _logger = logger;
        }

        public EntitiesDB entitiesDB { get; set; } = null!;

        public void Ready()
        {
            // throw new NotImplementedException();
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<T> entities, ExclusiveGroupStruct groupID)
        {
            var (components, egids, _) = entities;
            var (ids, _) = this.entitiesDB.QueryEntities<EntityId>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
#if DEBUG
                _logger.Verbose("{ClassName}<{TName}>::{MethodName} - Disposing of {ComponentType} for {VhId}", nameof(InstanceDisposableEngine<T>), _tName, nameof(Remove), _tName, ids[index].VhId.Value);
#endif
                components[index].Dispose();
            }
        }
    }
}


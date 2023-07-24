using Svelto.ECS;
using Serilog;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    internal sealed class DisposableEngine<T> : IEngine, IReactOnRemoveEx<T>, IQueryingEntitiesEngine
        where T : unmanaged, IEntityComponent, IDisposable
    {
        private readonly ILogger _logger;

        public DisposableEngine(ILogger logger)
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
                _logger.Verbose("{ClassName}::{MethodName} - Disposing of {ComponentType} for {VhId}", nameof(DisposableEngine<T>), nameof(Remove), typeof(T).Name, ids[index].VhId.Value);
                components[index].Dispose();
            }
        }
    }
}

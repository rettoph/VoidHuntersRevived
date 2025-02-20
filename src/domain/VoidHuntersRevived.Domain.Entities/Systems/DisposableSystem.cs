using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Logging.Common;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    public sealed class DisposableSystem<T>(EntitiesDB entitiesDb, ILogger logger) : ISceneSystem, IEngine, IReactOnRemoveEx<T>
        where T : unmanaged, IEntityComponent, IDisposable
    {
        private static readonly string _tName = typeof(T).GetFormattedName();

        private readonly EntitiesDB _entitiesDb = entitiesDb;
        private readonly ILogger _logger = logger;

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<T> entities, ExclusiveGroupStruct groupID)
        {
            var (components, egids, _) = entities;
            var (localIds, _) = this._entitiesDb.QueryEntities<EntityLocalId>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                this._logger.Verbose("{ClassName}<{TName}>::{MethodName} - Disposing of {ComponentType} for {LocalId}", nameof(DisposableSystem<T>), _tName, nameof(Remove), _tName, localIds[index]);
                components[index].Dispose();
            }
        }
    }
}
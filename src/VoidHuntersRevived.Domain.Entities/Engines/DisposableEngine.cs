using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common.Components;
using Serilog;

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
            var (vhids, _) = this.entitiesDB.QueryEntities<EntityVhId>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                _logger.Verbose("{ClassName}::{MethodName} - Disposing of {ComponentType} for {VhId}", nameof(DisposableEngine<T>), nameof(Remove), typeof(T).Name, vhids[index].Value.Value);
                components[index].Dispose();
            }
        }
    }
}

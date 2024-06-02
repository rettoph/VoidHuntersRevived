using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    /// <summary>
    /// Magic engine that will handle adding and removing new entities with <see cref="BelongsTo{,}"/> components
    /// to their respective parent filters.
    /// 
    /// Instances of this engine are automatically created within the <see cref="Providers.BelongsToEngineProvider"/>
    /// </summary>
    /// <typeparam name="TOwner"></typeparam>
    /// <typeparam name="TItems"></typeparam>
    [Sequence<EngineSequence>(EngineSequence.Group01)]
    internal sealed class BelongsToEngine<TOwner, TItems> : BasicEngine, IOnSpawnEngine<BelongsTo<TOwner, TItems>>
        where TOwner : unmanaged, IEntityComponent
        where TItems : unmanaged, IEntityComponent
    {
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public BelongsToEngine(IEntityService entities, ILogger logger)
        {
            _entities = entities;
            _logger = logger;
        }

        public void OnSpawn(VhId sourceEventId, IEntityType type, EntityId id, ref BelongsTo<TOwner, TItems> belongsTo, in GroupIndex groupIndex)
        {
            if (belongsTo.OwnerVhId == default)
            {
                _logger.Warning("{0}::{1} - Empty OwnerId", typeof(BelongsToEngine<TOwner, TItems>).GetFormattedName(), nameof(BelongsToEngine<TOwner, TItems>.OnSpawn));
                return;
            }

            EntityId ownerId = _entities.GetId(belongsTo.OwnerVhId);
            HasMany<TItems, TOwner> hasMany = _entities.QueryById<HasMany<TItems, TOwner>>(ownerId);
            hasMany.Items.Add(id, groupIndex);
        }
    }
}

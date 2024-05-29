using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
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
    internal sealed class BelongsToEngine<TOwner, TItems> : BasicEngine, IReactOnAddEx<BelongsTo<TOwner, TItems>>
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

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<BelongsTo<TOwner, TItems>> entities, ExclusiveGroupStruct groupID)
        {
            var (belongsTos, nativeIds, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                BelongsTo<TOwner, TItems> belongsTo = belongsTos[index];
                if (belongsTo.OwnerId == default)
                {
                    _logger.Warning("{0}::{1} - Empty OwnerId", typeof(BelongsToEngine<TOwner, TItems>).GetFormattedName(), nameof(BelongsToEngine<TOwner, TItems>.Add));
                    continue;
                }

                HasMany<TItems, TOwner> hasMany = _entities.QueryById<HasMany<TItems, TOwner>>(belongsTo.OwnerId);

                hasMany.Items.Add(nativeIds[index], groupID, index);
            }
        }
    }
}

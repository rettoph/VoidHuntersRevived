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
    /// <typeparam name="T"></typeparam>
    internal sealed class BelongsToEngine<TOwner, T> : BasicEngine, IReactOnAddEx<BelongsTo<TOwner, T>>
        where TOwner : unmanaged, IEntityComponent
        where T : unmanaged, IEntityComponent
    {
        private readonly IEntityService _entities;

        public BelongsToEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<BelongsTo<TOwner, T>> entities, ExclusiveGroupStruct groupID)
        {
            var (belongsTos, egids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                BelongsTo<TOwner, T> belongsTo = belongsTos[index];
                HasMany<T, TOwner> hasMany = _entities.QueryById<HasMany<T, TOwner>>(belongsTo.OwnerId);

                hasMany.Items.Add(egids[index], groupID, index);
            }
        }
    }
}

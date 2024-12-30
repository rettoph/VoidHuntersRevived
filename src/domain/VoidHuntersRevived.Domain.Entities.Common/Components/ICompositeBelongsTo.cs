using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    /// <summary>
    /// Simple interface that will automatically add any entities containing both
    /// <typeparamref name="TPrimary"/> and <typeparamref name="TSecondary"/> to a filter.
    /// <typeparamref name="TPrimary"/> must be a functional <see cref="IBelongsTo{TParent, TOriginal}"/>
    /// and acts as the link between the current component instance and the <typeparamref name="TParent"/> instance.
    /// 
    /// This can be simply thought of as a way to make instance filters more granular at runtime. There is a special
    /// engine within domain responseible for adding the required functionality to the game.
    /// 
    /// The composite filter can be accessed via <see cref="Services.IEntityQueryService.GetCompositeFilter{TComponent1, TComponent2}(EntityLocalId){T}(EGID, FilterContextID)"/>
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    /// <typeparam name="TSecondary"></typeparam>
    public interface ICompositeBelongsTo<TParent, TPrimary, TSecondary> : IEntityComponent
        where TParent : unmanaged, IEntityComponent, IHasMany<TPrimary>
        where TPrimary : unmanaged, IBelongsTo<TParent, TPrimary>
        where TSecondary : unmanaged, ICompositeBelongsTo<TParent, TPrimary, TSecondary>, IEntityComponent
    {
    }
}

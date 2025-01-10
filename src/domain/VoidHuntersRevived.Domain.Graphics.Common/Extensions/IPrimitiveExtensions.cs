using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Graphics.Common.Extensions
{
    public static class IPrimitiveExtensions
    {
        public static ref EntityFilterCollection GetFilter<TVertex>(this IPrimitive primitive, EntitiesDB entitiesDb)
            where TVertex : unmanaged, IVertexType, IEntityComponent => ref entitiesDb.GetFilters().GetOrCreatePersistentFilter<TVertex>(primitive.CombinedFilterId);
    }
}
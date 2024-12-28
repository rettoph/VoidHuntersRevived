using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Common.Extensions
{
    public static class IInputDataExtensions
    {
        public delegate void ProcessInputDelegate<TComponent>(Entity<TComponent> entity)
            where TComponent : unmanaged, IEntityComponent;

        public static void Process<TData, TComponent>(this TData data, IEntityQueryService entityQueryService)
            where TComponent : unmanaged, IEntityComponent
        {

        }
    }
}

using Guppy.Core.Common.Providers;
using Guppy.Core.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Tests.Common.Simulations.Extensions
{
    public static class SystemFactoriesExtensions
    {
        public static List<Func<TStrategy, IScopedSystem>> AddProviderSystems<TStrategy>(
            this List<Func<TStrategy, IScopedSystem>> systemFactories,
            params IScopedSystemProvider[] systemProviders)
                where TStrategy : IStrategy
        {
            foreach (IScopedSystem system in systemProviders.SelectMany(x => x.GetSystems()))
            {
                systemFactories.Add(x => system);
            }

            return systemFactories;
        }
    }
}

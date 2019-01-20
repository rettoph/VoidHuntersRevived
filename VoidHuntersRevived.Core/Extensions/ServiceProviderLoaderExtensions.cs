using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Extensions
{
    public static class ServiceProviderLoaderExtensions
    {
        public static TLoader GetLoader<TLoader>(this IServiceProvider provider)
            where TLoader : class, ILoader
            => (TLoader)provider.GetServices<ILoader>()
                .Where(l => l.GetType() == typeof(TLoader))
                .FirstOrDefault();
    }
}

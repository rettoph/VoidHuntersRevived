using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Extensions
{
    public static class ServiceCollectionLoaderExtensions
    {
        public static void AddLoader<TLoader>(this IServiceCollection services)
            where TLoader : class, ILoader
            => services.AddSingleton<ILoader, TLoader>();
    }
}

using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Loaders
{
    [AutoLoad]
    public sealed class PieceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddSingleton<PieceConfigurationService>()
                    .AddInterfaceAliases();

                manager.AddSingleton<PieceCategoryService>()
                    .AddInterfaceAliases();

                manager.AddSingleton<PiecePropertyService>()
                    .AddInterfaceAliases();

                manager.AddSingleton<PieceService>()
                    .AddInterfaceAliases();
            });
        }
    }
}

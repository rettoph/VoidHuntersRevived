using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Factories;
using VoidHuntersRevived.Common.ECS.Services;
using VoidHuntersRevived.Domain.ECS.Factories;
using VoidHuntersRevived.Domain.ECS.Services;

namespace VoidHuntersRevived.Domain.ECS.Loaders
{
    [AutoLoad]
    public sealed class ECSLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddTransient<IWorldFactory>()
                    .SetImplementationType<WorldFactory>();

                manager.AddScoped<EntityTypeService>()
                    .AddAlias<IEntityTypeService>();
            });
        }
    }
}

using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddSingleton<EntityTypeService>()
                    .AddAlias<IEntityTypeService>();

                manager.AddScoped<SimpleEntitiesSubmissionScheduler>()
                    .AddAlias<EntitiesSubmissionScheduler>();

                manager.AddScoped<EnginesRoot>();

                manager.AddScoped<EngineService>()
                    .AddInterfaceAliases();

                manager.AddScoped<EntityService>()
                    .AddInterfaceAliases();

                manager.AddScoped<EntitySerializationService>()
                    .AddInterfaceAliases();

                manager.AddScoped<EventPublishingService>()
                    .AddInterfaceAliases();

                manager.AddScoped<FilterService>()
                    .AddInterfaceAliases();
            });
        }
    }
}

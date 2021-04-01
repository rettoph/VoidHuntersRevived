using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Lists;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class TeamsServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            services.AddFactory<TeamList>(p => new TeamList());
            services.AddScoped<TeamList>();

            services.AddFactory<Team>(p => new Team());
            services.AddTransient<Team>();

            services.AddSetup<Team>((team, p,s) =>
            {
                p.GetService<TeamList>().TryAdd(team);
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Domain.Server.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<ServerGameGuppy>();

            services.ConfigureCollection(manager =>
            {
 
            });
        }
    }
}

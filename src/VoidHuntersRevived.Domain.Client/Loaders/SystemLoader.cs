using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Domain.Client.Options;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class SystemLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
            });

            services.Configure<DrawableOptions<Lockstep>>(options =>
            {
                options.Visible = false;
                options.Tint = Color.Red;
            });

            services.Configure<DrawableOptions<Predictive>>(options =>
            {
                options.Visible = true;
                options.Tint = null;
            });
        }
    }
}

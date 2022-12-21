using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library.Services;

namespace VoidHuntersRevived.Client.Library.Loaders
{
    [AutoLoad(0)]
    internal sealed class DebugServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            services.AddScoped<AetherBodyPositionDebugService>();
#endif
        }
    }
}

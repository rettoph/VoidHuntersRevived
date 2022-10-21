using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Server.Constants;

namespace VoidHuntersRevived.Server.Loaders
{
    [AutoLoad]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<ServerMainGuppy>()
                    .AddGuppy<ServerGameGuppy>();

            services.AddSetting<int>(SettingConstants.Port, 1337, true);
        }
    }
}

using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class SettingServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSetting<int>(SettingConstants.TickSpeed, 68, false);
            services.AddSetting<int>(SettingConstants.WorldStepsPerTick, 4, false);
        }
    }
}

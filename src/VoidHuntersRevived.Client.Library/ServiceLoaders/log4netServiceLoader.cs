using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.log4net;
using Guppy.Interfaces;
using Guppy.IO.Extensions.log4net;
using log4net;
using log4net.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class log4netServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterSetup<ILog>((l, p, s) =>
            {
                l.SetLevel(Level.Verbose);
                l.ConfigureTerminalAppender(
                    p,
                    (Level.Fatal, Color.Red),
                    (Level.Error, Color.Red),
                    (Level.Warn, Color.Yellow),
                    (Level.Info, Color.White),
                    (Level.Debug, Color.Magenta),
                    (Level.Verbose, Color.Cyan)
                );
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

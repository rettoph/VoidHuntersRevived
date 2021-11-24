using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Extensions.log4net;
using log4net;
using log4net.Appender;
using log4net.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.log4net;

namespace VoidHuntersRevived.Server.ServiceLoaders
{
    [AutoLoad]
    internal sealed class log4netServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterSetup<ILog>((l, p, s) =>
            {
                l.SetLevel(Level.Verbose);
                l.ConfigureFileAppender($"logs\\{DateTime.Now.ToString("yyy-MM-dd")}.txt")
                    .ConfigureManagedColoredConsoleAppender(new ManagedColoredConsoleAppender.LevelColors()
                    {
                        BackColor = ConsoleColor.Red,
                        ForeColor = ConsoleColor.White,
                        Level = Level.Fatal
                    }, new ManagedColoredConsoleAppender.LevelColors()
                    {
                        ForeColor = ConsoleColor.Red,
                        Level = Level.Error
                    }, new ManagedColoredConsoleAppender.LevelColors()
                    {
                        ForeColor = ConsoleColor.Yellow,
                        Level = Level.Warn
                    }, new ManagedColoredConsoleAppender.LevelColors()
                    {
                        ForeColor = ConsoleColor.White,
                        Level = Level.Info
                    }, new ManagedColoredConsoleAppender.LevelColors()
                    {
                        ForeColor = ConsoleColor.Magenta,
                        Level = Level.Debug
                    }, new ManagedColoredConsoleAppender.LevelColors()
                    {
                        ForeColor = ConsoleColor.Cyan,
                        Level = Level.Verbose
                    });
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

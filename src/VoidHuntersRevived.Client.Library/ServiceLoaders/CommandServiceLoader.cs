using Guppy.Attributes;
using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class CommandServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterSetup<CommandService>((commands, _, _) =>
            {
                commands.Get().AddCommand(new Command("ship")
                {
                    new Command("set")
                    {
                        new Command("direction")
                        {
                            new Argument<Direction>("direction"),
                            new Argument("value"),
                        }
                    }
                });
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

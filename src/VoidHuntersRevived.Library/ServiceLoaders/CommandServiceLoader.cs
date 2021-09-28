using Guppy.Attributes;
using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.ServiceLoaders
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
                            new Argument("state"),
                        }
                    },
                    new Command("tractorbeam")
                    {
                        new Argument<TractorBeamActionType>("action"),
                        new Option<Single?>("--x", () => default, "Defaults to current mouse X position"),
                        new Option<Single?>("--y", () => default, "Defaults to current mouse Y position")
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

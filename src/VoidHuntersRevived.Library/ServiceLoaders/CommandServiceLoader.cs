using Guppy.Attributes;
using Guppy.CommandLine.Services;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Extensions.System;
using Guppy.Interfaces;
using Guppy.ServiceLoaders;
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
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            // services.RegisterSetup<CommandService>((commands, _, _) =>
            // {
            //     commands.Get().AddCommand(new Command("ship")
            //     {
            //         new Command("thrust")
            //         {
            //             new Argument<Direction>("direction"),
            //             new Argument("state"),
            //         },
            //         new Command("tractorbeam")
            //         {
            //             new Argument<TractorBeamActionType>("action"),
            //             new Option<Single?>("--x", () => default, "Defaults to current mouse X position"),
            //             new Option<Single?>("--y", () => default, "Defaults to current mouse Y position")
            //         }
            //     });
            // });
        }
    }
}

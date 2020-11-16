using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Commands.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Contexts;
using VoidHuntersRevived.Library.Events;
using VoidHuntersRevived.Library.Entities;
using System.Linq;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Client.Library.Enums;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class CommandServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            // throw new NotImplementedException();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            var commands = provider.GetService<CommandService>();

            // Add set commands...
            commands.TryAddCommand(new CommandContext()
            {
                Word = "ship",
                Description = "Manipulate the current user's ship.",
                Commands = new CommandContext[]
                {
                    new CommandContext()
                    {
                        Word = "direction",
                        Description = "Update the ship's direction.",
                        Arguments = new ArgContext[]
                        {
                            new ArgContext()
                            {
                                Identifier = "direction",
                                Aliases = "d".ToCharArray(),
                                Required = true,
                                Type = ArgType.FromEnum<Ship.Direction>()
                            },
                            new ArgContext()
                            {
                                Identifier = "value",
                                Aliases = "v".ToCharArray(),
                                Required = true,
                                Type = ArgType.Boolean
                            }
                        }
                    },
                    new CommandContext()
                    {
                        Word = "save",
                        Description = "Save the ship data to file.",
                        Arguments = new ArgContext[]
                        {
                            new ArgContext()
                            {
                                Identifier = "name",
                                Required = true,
                                Aliases = "n".ToCharArray(),
                                Type = ArgType.String
                            }
                        }
                    },
                    new CommandContext()
                    {
                        Word = "tractorbeam",
                        Description = "Manipulate the ship's tractorbeam.",
                        Arguments = new ArgContext[]
                        {
                            new ArgContext()
                            {
                                Identifier = "action",
                                Required = true,
                                Aliases = "a".ToCharArray(),
                                Type = ArgType.FromEnum<TractorBeam.ActionType>()
                            }
                        }
                    }
                }
            });

            // Add toggle commands...
            commands.TryAddCommand(new CommandContext()
            {
                Word = "toggle",
                Commands = new CommandContext[]
                {
                    new CommandContext()
                    {
                        Word = "debug",
                        Arguments = new ArgContext[]
                        {
                            new ArgContext()
                            {
                                Identifier = "type",
                                Required = true,
                                Aliases = "t".ToCharArray(),
                                Type = ArgType.FromEnum<DebugType>()
                            }
                        }
                    }
                }
            });
        }
    }
}

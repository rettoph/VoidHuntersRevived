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
        public void ConfigureServices(ServiceCollection services)
        {
            // throw new NotImplementedException();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            var commands = provider.GetService<CommandService>();

            // Add set commands...
            commands.TryAddSubCommand(new CommandContext()
            {
                Word = "set",
                SubCommands = new CommandContext[]
                {
                    new CommandContext()
                    {
                        Word = "direction",
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
                    }
                }
            });

            // Add tractor beam commands...
            commands.TryAddSubCommand(new CommandContext()
            {
                Word = "tractorbeam",
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
            });

            // Add save commands...
            commands.TryAddSubCommand(new CommandContext()
            {
                Word = "save",
                SubCommands = new CommandContext[]
                {
                    new CommandContext()
                    {
                        Word = "ship",
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
                    }
                }
            });

            // Add toggle commands...
            commands.TryAddSubCommand(new CommandContext()
            {
                Word = "toggle",
                SubCommands = new CommandContext[]
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

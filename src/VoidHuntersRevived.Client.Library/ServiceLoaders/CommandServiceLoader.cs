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
using VoidHuntersRevived.Library.Entities;
using System.Linq;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Windows.Library.Enums;
using System.IO;

namespace VoidHuntersRevived.Windows.Library.ServiceLoaders
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
                        Word = "fire",
                        Description = "Set the ship's firing status.",
                        Arguments = new ArgContext[]
                        {
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
                    },
                    new CommandContext()
                    {
                        Word = "self-destruct",
                        Description = "Self destruct the current ship."
                    },
                    new CommandContext()
                    {
                        Word = "launch-fighters",
                        Description = "Attempt to launch the current ship's fighter bays (if any)."
                    },
                    new CommandContext()
                    {
                        Word = "toggle-energy-shields",
                        Description = "Attempt to toggle the current ship's energy shields (if any)."
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

            // Add spawn commands...
            commands.TryAddCommand(new CommandContext()
            {
                Word = "spawn",
                Description = "Attempt to spawn an item into the game",
                Commands = new CommandContext[] {
                    new CommandContext()
                    {
                        Word = "ai",
                        Description = "Attempt to spawn an AI instance into the game",
                        Arguments = new ArgContext[]
                        {
                            new ArgContext()
                            {
                                Identifier = "name",
                                Aliases = "n".ToCharArray(),
                                Description = "The name of the ship to spawn. When empty, a random ship will be used instead.",
                                Type = new ArgType("File Name", name => File.OpenRead($"Resources/Ships/{name}.vh"))
                            },
                            new ArgContext()
                            {
                                Identifier = "positionX",
                                Aliases = "x".ToCharArray(),
                                Description = "The X position at which to spawn the AI. If not defined, the cursor position will be used instead.",
                                Type = ArgType.Single
                            },
                            new ArgContext()
                            {
                                Identifier = "positionY",
                                Aliases = "y".ToCharArray(),
                                Description = "The Y position at which to spawn the AI. If not defined, the cursor position will be used instead.",
                                Type = ArgType.Single
                            },
                            new ArgContext()
                            {
                                Identifier = "rotation",
                                Aliases = "r".ToCharArray(),
                                Description = "The rotation at which to spawn the AI. If not defined, a random value will be used instead.",
                                Type = ArgType.Single
                            }
                        }
                    }
                }
            });
        }
    }
}

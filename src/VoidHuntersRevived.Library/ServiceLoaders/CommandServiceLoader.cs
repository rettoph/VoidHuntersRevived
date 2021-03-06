﻿using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Contexts;
using Guppy.IO.Commands.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VoidHuntersRevived.Library.ServiceLoaders
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
                Word = "world",
                Description = "Manipulate the local world instance.",
                Commands = new CommandContext[]
                {
                    new CommandContext()
                    {
                        Word = "info",
                        Description = "Display a snapshot of the current world state.",
                        Arguments = new ArgContext[]
                        {
                            new ArgContext()
                            {
                                Identifier = "depth",
                                Aliases = "d".ToCharArray(),
                                DefaultValue = () => 5,
                                Type = ArgType.Int32,
                                Description = "The precision depth to dump."
                            },
                            new ArgContext()
                            {
                                Identifier = "members",
                                Aliases = "m".ToCharArray(),
                                DefaultValue = () => MemberTypes.Property | MemberTypes.Field,
                                Type = ArgType.FromEnum<MemberTypes>(),
                                Description = "The members to be dumped."
                            }
                        }
                    }
                }
            });
        }
    }
}

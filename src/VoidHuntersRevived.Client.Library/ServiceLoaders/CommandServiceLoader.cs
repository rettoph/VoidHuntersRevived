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
            provider.GetService<CommandService>().TryAddSubCommand(new CommandContext()
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
                                Type = new ArgType("Direction", s => Enum.Parse(typeof(Ship.Direction), s, true), ((Ship.Direction[])Enum.GetValues(typeof(Ship.Direction))).Select(d => d.ToString().ToLower()).ToArray())
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
        }
    }
}

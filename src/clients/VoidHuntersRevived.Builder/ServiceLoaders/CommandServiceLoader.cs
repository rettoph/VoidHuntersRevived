using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Contexts;
using Guppy.IO.Commands.Services;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.Enums;

namespace VoidHuntersRevived.Builder.ServiceLoaders
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
                Word = "lock",
                Arguments = new ArgContext[]
                {
                    new ArgContext()
                    {
                        Identifier = "type",
                        Aliases = "t".ToCharArray(),
                        Type = ArgType.FromEnum<LockType>(),
                        Description = "The lock type."
                    },
                    new ArgContext()
                    {
                        Identifier = "value",
                        Aliases = "v".ToCharArray(),
                        DefaultValue = () => 5,
                        Type = ArgType.Boolean,
                        Description = "The target lock value."
                    }
                }
            });
        }
    }
}

using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Commands.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using Guppy.IO.Commands;
using VoidHuntersRevived.Client.Library.Commands;

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
            provider.GetService<CommandService>().TryAdd(new SegmentContext()
            {
                Identifier = "set",
                SubSegments = new SegmentContext[]
                {
                    new SegmentContext()
                    {
                        Identifier = "direction",
                        Command = new SetDirectionCommandContext()
                    }
                }
            });
        }
    }
}

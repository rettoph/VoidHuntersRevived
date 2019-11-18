using Guppy.Attributes;
using Guppy.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;

namespace VoidHuntersRevived.Client.Library
{
    [IsServiceLoader]
    public class ClientServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}

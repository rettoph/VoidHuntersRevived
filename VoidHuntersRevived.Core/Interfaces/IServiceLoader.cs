using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IServiceLoader : IInitializable
    {
        IGame Game { get; }

        event EventHandler<IServiceLoader> OnConfigureServices;

        void TryConfigureServices(IServiceCollection services);
    }
}

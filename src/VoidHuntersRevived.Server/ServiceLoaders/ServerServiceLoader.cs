using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Utilities;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ServerServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            services.AddGame<ServerVoidHuntersRevivedGame>(p => new ServerVoidHuntersRevivedGame());
            services.AddScene<GameScene>(p => new ServerGameScene(), 1);

            services.AddConfiguration<Settings>((s, p, c) =>
            { // Configure the server settings...
                s.Set<GameAuthorization>(GameAuthorization.Full);
            }, 1);
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // Configure the console logging component...
            provider.GetService<Logger>().ConfigureConsoleLogging();
        }
    }
}

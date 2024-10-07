using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Engine.Common.Loaders;
using Guppy.Game;
using Guppy.Game.MonoGame.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.Configure<ISceneConfiguration<IStrategy>>((scope, configuration) =>
            {
                configuration.SetSceneHasDebugWindow(true).SetSceneHasTerminalWindow(true);
            });
        }
    }
}

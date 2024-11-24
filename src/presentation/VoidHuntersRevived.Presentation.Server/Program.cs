using Autofac;
using Guppy.Core.Commands.Common.Services;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Extensions;
using Guppy.Game;
using Guppy.Game.Common.Extensions;
using Guppy.Game.Console.Extensions;
using Guppy.Game.Helpers;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Game.Server;
using VoidHuntersRevived.Presentation.Client;
using VoidHuntersRevived.Presentation.Core;

var engine = new GameEngine(VoidHuntersContextBuilder.ServerContext, builder =>
{
    builder.RegisterConsoleGameServices().RegisterCoreNetworkServices();
    builder.RegisterType<ServerSerilogSinkConfigurator>().As<ISerilogSinkConfigurator>().InstancePerLifetimeScope();
}).Start();

AppDomain.CurrentDomain.ProcessExit += new EventHandler((sender, args) =>
{
    engine.Dispose();
});

engine.Scenes.Create<ServerGameScene>(configuration =>
{
    configuration.WithContainerBuilder(builder =>
    {
        builder.RegisterNetScope<IStrategy>(PeerType.Server, NetScopeIds.Game);
    });
});

var source = new CancellationTokenSource();
_ = TaskHelper.CreateLoop(
    engine.Update,
    TimeSpan.FromMilliseconds(16),
    source.Token
);

while (true)
{
    string? input = Console.ReadLine();

    if (input is null)
    {
        continue;
    }

    engine.Scenes.GetAll().Last().Resolve<ICommandService>().Invoke(input);
}

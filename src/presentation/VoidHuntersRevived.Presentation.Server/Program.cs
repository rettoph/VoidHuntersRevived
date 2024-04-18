using Autofac;
using Guppy.Core.Commands.Common.Services;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Extensions;
using Guppy.Engine;
using Guppy.Game.Common.Extensions;
using Guppy.Game.Console.Extensions;
using Guppy.Game.Helpers;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Game.Server;
using VoidHuntersRevived.Presentation.Core;

var game = GuppyEngine.Start(VoidHuntersContextBuilder.ClientContext, builder =>
{
    builder.RegisterConsoleGameServices().RegisterCoreNetworkServices();
}).StartGame();

AppDomain.CurrentDomain.ProcessExit += new EventHandler((sender, args) =>
{
    game.Dispose();
});

game.Guppies.Create<ServerGameGuppy>(builder =>
{
    builder.RegisterNetScope<ISimulation>(PeerType.Server, NetScopeIds.Game);
});

var source = new CancellationTokenSource();
_ = TaskHelper.CreateLoop(
    game.Update,
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

    game.Guppies.Last().Scope.Resolve<ICommandService>().Invoke(input);
}

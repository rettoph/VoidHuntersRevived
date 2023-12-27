using Autofac;
using Guppy;
using Guppy.Commands.Services;
using Guppy.Game.Extensions;
using Guppy.Game.Helpers;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Server;

var game = new GuppyEngine(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}.Server").StartGame();
AppDomain.CurrentDomain.ProcessExit += new EventHandler((sender, args) =>
{
    game.Dispose();
});

game.Guppies.Create<ServerGameGuppy>();

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

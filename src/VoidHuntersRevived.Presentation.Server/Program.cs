using Guppy;
using Guppy.Game.Extensions;
using Guppy.Game.Helpers;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Server;

var game = new GuppyEngine(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}.Server").StartGame();

game.Guppies.Create<ServerGameGuppy>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(
    game.Update,
    TimeSpan.FromMilliseconds(16),
    source.Token
).GetAwaiter().GetResult();

Console.ReadLine();
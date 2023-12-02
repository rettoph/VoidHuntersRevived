using Guppy.Common;
using Guppy;
using Guppy.Providers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Game;
using VoidHuntersRevived.Game.Server;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Game.Common;
using Guppy.Game;
using Guppy.Game.Helpers;
using Guppy.Game.Extensions;

var game = new GuppyEngine(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}.Server").StartGame();

game.Guppies.Create<ServerGameGuppy>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(
    game.Update, 
    TimeSpan.FromMilliseconds(16), 
    source.Token
).GetAwaiter().GetResult();

Console.ReadLine();
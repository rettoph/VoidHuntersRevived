using Guppy.Common;
using Guppy.MonoGame.Helpers;
using Guppy;
using Guppy.Providers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Game;
using VoidHuntersRevived.Game.Server;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Game.Common;
using Guppy.MonoGame.Common;
using Guppy.MonoGame.Extensions;

var game = new GuppyEngine(VoidHuntersRevivedGame.Company, VoidHuntersRevivedGame.Name, new[] { typeof(GameGuppy).Assembly, typeof(ServerGameGuppy).Assembly, typeof(IGameGuppy).Assembly, typeof(VoidHuntersEntityDescriptor).Assembly })
    .StartGame(builder =>
    {
        builder.ConfigureGame()
            .ConfigureNetwork()
            .ConfigureResources();
    });

game.Guppies.Create<ServerGameGuppy>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(
    game.Update, 
    TimeSpan.FromMilliseconds(16), 
    source.Token
).GetAwaiter().GetResult();

Console.ReadLine();
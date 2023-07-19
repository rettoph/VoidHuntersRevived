using Guppy.Common;
using Guppy.MonoGame.Helpers;
using Guppy.MonoGame.Services;
using Guppy;
using Guppy.Providers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Game;
using VoidHuntersRevived.Game.Server;
using VoidHuntersRevived.Common;

var engine = new GuppyEngine(new[] { typeof(GameGuppy).Assembly, typeof(ServerGameGuppy).Assembly, typeof(IGameGuppy).Assembly })
    .Start(builder =>
    {
        builder.ConfigureGame()
            .ConfigureNetwork()
            .ConfigureResources();
    });

var guppy = (IUpdateable)engine.Guppies.Create<ServerGameGuppy>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(
    guppy.Update, 
    TimeSpan.FromMilliseconds(16), 
    source.Token
).GetAwaiter().GetResult();

Console.ReadLine();
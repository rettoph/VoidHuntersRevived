using Guppy.Common;
using Guppy.MonoGame.Helpers;
using Guppy.MonoGame.Services;
using Guppy;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Domain;
using VoidHuntersRevived.Domain.Server;
using Guppy.Providers;
using Microsoft.Xna.Framework;

var engine = new GuppyEngine(new[] { typeof(GameGuppy).Assembly, typeof(ServerGameGuppy).Assembly })
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
using Guppy.Common;
using Guppy.MonoGame.Helpers;
using Guppy.MonoGame.Services;
using Guppy;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Domain;
using VoidHuntersRevived.Domain.Server;
using Guppy.Providers;

var engine = new GuppyEngine(new[] { typeof(GameGuppy).Assembly, typeof(ServerGameGuppy).Assembly })
    .Start(builder =>
    {
        builder.ConfigureGame()
            .ConfigureECS()
            .ConfigureNetwork()
            .ConfigureResources();
    });

var guppy = engine.Guppies.Create<ServerGameGuppy>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(
    guppy.Instance.Update, 
    TimeSpan.FromMilliseconds(16), 
    source.Token
).GetAwaiter().GetResult();

Console.ReadLine();
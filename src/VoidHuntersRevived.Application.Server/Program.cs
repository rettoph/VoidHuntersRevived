using Guppy.Common;
using Guppy.MonoGame.Helpers;
using Guppy.MonoGame.Services;
using Guppy.MonoGame.Strategies.PublishStrategies;
using Guppy;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Server;
using Guppy.Providers;

var guppy = new GuppyEngine(new[] { typeof(GameGuppy).Assembly, typeof(ServerGameGuppy).Assembly })
    .ConfigureGame<LastGuppyPublishStrategy>()
    .ConfigureECS()
    .ConfigureNetwork()
    .ConfigureResources()
    .Build()
    .GetRequiredService<IGuppyProvider>()
    .Create<ServerGameGuppy>();

var globals = guppy.Scope.ServiceProvider.GetRequiredService<IGlobal<IGameComponentService>>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(gt =>
{
    globals.Instance.Update(gt);

    guppy.Instance.Update(gt);
}, TimeSpan.FromMilliseconds(16), source.Token).GetAwaiter().GetResult();

Console.ReadLine();
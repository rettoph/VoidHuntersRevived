using Guppy;
using Guppy.Common;
using Guppy.MonoGame.Helpers;
using Guppy.MonoGame.Services;
using Guppy.MonoGame.Strategies.PublishStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Server;

var guppy = new GuppyEngine(new[] { typeof(MainGuppy).Assembly, typeof(ServerMainGuppy).Assembly })
    .ConfigureGame<LastGuppyPublishStrategy>()
    .ConfigureECS()
    .ConfigureNetwork(3)
    .ConfigureResources()
    .Build()
    .GetRequiredService<IScoped<ServerMainGuppy>>();

var globals = guppy.Scope.ServiceProvider.GetRequiredService<IGlobal<IGameComponentService>>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(gt =>
{
    globals.Instance.Update(gt);

    guppy.Instance.Update(gt);
}, TimeSpan.FromMilliseconds(16), source.Token).GetAwaiter().GetResult();

Console.ReadLine();
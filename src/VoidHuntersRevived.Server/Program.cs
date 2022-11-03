using Guppy;
using Guppy.Common;
using Guppy.MonoGame.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Server;

var guppy = new GuppyEngine(new[] { typeof(MainGuppy).Assembly, typeof(ServerMainGuppy).Assembly })
    .ConfigureGame()
    .ConfigureECS()
    .ConfigureNetwork(1)
    .ConfigureResources()
    .Build()
    .Create<ServerMainGuppy>();

var globals = guppy.Scope.ServiceProvider.GetRequiredService<IGlobal<World>>();

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(gt =>
{
    globals.Instance.Update(gt);

    guppy.Instance.Update(gt);
}, TimeSpan.FromMilliseconds(16), source.Token).GetAwaiter().GetResult();

Console.ReadLine();
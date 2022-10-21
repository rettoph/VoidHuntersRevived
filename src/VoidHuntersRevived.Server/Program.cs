using Guppy;
using Guppy.MonoGame.Helpers;
using Microsoft.Xna.Framework;
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

var source = new CancellationTokenSource();

TaskHelper.CreateLoop(gt =>
{
    guppy.Instance.Update(gt);
}, TimeSpan.FromMilliseconds(16), source.Token).GetAwaiter().GetResult();

Console.ReadLine();
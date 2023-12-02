using Autofac;
using Guppy.Common;
using System.Numerics;
using System.Runtime;
using System.Text.Json;
using VoidHuntersRevived.Application.Client;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint.Extensions;

// using (var game = new VoidHuntersGame())
//     game.Run();

GCLatencyMode oldMode = GCSettings.LatencyMode;
Console.ResetColor();

try
{
    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    //GC.TryStartNoGCRegion(256 * 1024 * 1024);
    using (var game = new VoidHuntersGame())
        game.Run();
}
finally
{
    //GC.EndNoGCRegion();
    GCSettings.LatencyMode = oldMode;
}

class Test
{ }
using System.Runtime;
using VoidHuntersRevived.Application.Client;

// using (var game = new VoidHuntersGame())
//     game.Run();

GCLatencyMode oldMode = GCSettings.LatencyMode;
Console.ResetColor();

try
{
    Console.WriteLine("Enter to begin");
    Console.ReadLine();

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
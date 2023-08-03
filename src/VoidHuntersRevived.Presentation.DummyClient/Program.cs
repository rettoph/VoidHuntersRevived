using VoidHuntersRevived.Application.Client;

Console.WriteLine("Press enter to begin");
Console.ReadLine();

using (var game = new VoidHuntersGame())
    game.Run();
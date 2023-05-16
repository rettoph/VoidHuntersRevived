using VoidHuntersRevived.Application.Client;

Console.ResetColor();

Thread.Sleep(1000);

using (var game = new VoidHuntersGame())
    game.Run();
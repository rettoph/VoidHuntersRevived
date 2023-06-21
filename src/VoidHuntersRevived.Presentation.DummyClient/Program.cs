using VoidHuntersRevived.Application.Client;

Thread.Sleep(5000);

using (var game = new VoidHuntersGame())
    game.Run();
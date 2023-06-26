using VoidHuntersRevived.Application.Client;

Thread.Sleep(50);

using (var game = new VoidHuntersGame())
    game.Run();
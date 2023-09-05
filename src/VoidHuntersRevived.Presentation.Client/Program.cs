using VoidHuntersRevived.Application.Client;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.FixedPoint.Utilities;

Console.ResetColor();

using (var game = new VoidHuntersGame())
    game.Run();
﻿using VoidHuntersRevived.Client.Library;
using System;
using System.Threading;
using VoidHuntersRevived.Library.Extensions.System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Client.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Host:");
            var game = new Game1("localhost");
            game.Run();
        }
    }
}

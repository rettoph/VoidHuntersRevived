﻿using GalacticFighters.Client.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticFighters.Client.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game1();
            game.Run();
        }
    }
}

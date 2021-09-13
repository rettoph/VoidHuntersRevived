using Guppy;
using Guppy.Extensions;
using Guppy.Network.Extensions;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using Guppy.Extensions.System;
using VoidHuntersRevived.Library;
using tainicom.Aether.Physics2D.Dynamics;
using System.Threading;
using System.Linq;

namespace VoidHuntersRevived.Server
{
    class Program
    {      
        static void Main(string[] args)
        {
            var game = new GuppyLoader()
                .Initialize()
                .BuildGame<PrimaryGame>();
           

            game.TryStart(false);
        }
    }
}

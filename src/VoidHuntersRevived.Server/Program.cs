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
using System.Threading.Tasks;

namespace VoidHuntersRevived.Server
{
    class Program
    {      
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var game = new GuppyLoader()
                .Initialize()
                .BuildGame<PrimaryGame>();

            await game.TryStartAsync(false);
        }
    }
}

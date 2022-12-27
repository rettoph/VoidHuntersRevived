using Guppy.Common;
using Guppy.MonoGame.Services;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Services;
using Guppy.Resources.Serialization.Json;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Server
{
    public sealed class ServerGameGuppy : GameGuppy
    {
        public ServerGameGuppy(SimulationState state, NetScope netScope, LockstepSimulation lockstepSimulation, IJsonSerializer serializer, IGameComponentService components) : base(netScope, lockstepSimulation, components)
        {
            Console.WriteLine("Server Running!");

            string content = File.ReadAllText("replay.vhr");
            var history = serializer.Deserialize<IList<Tick>>(content);
            state.Read(history);
        }
    }
}

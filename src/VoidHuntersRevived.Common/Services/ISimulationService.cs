using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Services
{
    public interface ISimulationService
    {
        SimulationType Flags { get; }

        IEnumerable<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Initialize(SimulationType flags);

        void Update(GameTime gameTime);

        void PublishEvent(PeerType source, ISimulationData data);
    }
}

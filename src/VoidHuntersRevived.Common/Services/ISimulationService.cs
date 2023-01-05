using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Services
{
    public interface ISimulationService
    {
        ReadOnlyCollection<SimulationType> Types { get; }

        ReadOnlyCollection<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Initialize(params SimulationType[] simulationTypes);

        void Update(GameTime gameTime);

        void PublishEvent(PeerType source, ISimulationData data);
        bool Contains(SimulationType type);
    }
}

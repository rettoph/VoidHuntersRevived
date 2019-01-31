using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Interfaces
{
    public interface ITractorableEntity : IFarseerEntity, INetworkEntity
    {
        /// <summary>
        /// The current tractor beam connection object
        /// </summary>
        TractorBeamConnection TractorBeamConnection { get; }

        event EventHandler<ITractorableEntity> OnTractorBeamConnected;
        event EventHandler<ITractorableEntity> OnTractorBeamDisconnedted;

        Boolean CanBeSelectedBy(ITractorBeam tractorBeam);
        void Connect(TractorBeamConnection tractorBeamConnection);

        void DisconnectTractorBeam();
    }
}

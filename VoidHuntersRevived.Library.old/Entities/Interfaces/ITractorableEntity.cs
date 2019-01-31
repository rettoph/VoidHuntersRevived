using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Interfaces
{
    public interface ITractorableEntity : IFarseerEntity, INetworkEntity
    {
        /// <summary>
        /// The current tractor beam selecting the
        /// current tractorable entity (nullable)
        /// </summary>
        ITractorBeam TractorBeam { get; }

        event EventHandler<ITractorableEntity> OnSelected;
        event EventHandler<ITractorableEntity> OnReleased;

        Boolean CanBeSelectedBy(ITractorBeam tractorBeam);
        void SelectedBy(ITractorBeam tractorBeam);

        void Released();
    }
}

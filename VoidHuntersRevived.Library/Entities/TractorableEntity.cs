using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorableEntity : NetworkedFarseerEntity, ITractorableEntity
    {
        public TractorBeamConnection TractorBeamConnection { get; private set; }

        public event EventHandler<ITractorableEntity> OnTractorBeamConnected;
        public event EventHandler<ITractorableEntity> OnTractorBeamDisconnedted;

        public TractorableEntity(EntityInfo info, IGame game) : base(info, game)
        {
        }
        public TractorableEntity(Int64 id, EntityInfo info, IGame game) : base(id, info, game)
        {
        }


        public virtual Boolean CanBeSelectedBy(ITractorBeam tractorBeam)
        {
            return this.TractorBeamConnection == null;
        }

        public void DisconnectTractorBeam()
        {
            if (this.TractorBeamConnection.State != ConnectionState.Disconnecting)
                throw new Exception("Unable to disconnect! Tractor beam connection not set to disconnect.");

            this.TractorBeamConnection = null;
            this.Body.SleepingAllowed = true;

            this.OnTractorBeamDisconnedted?.Invoke(this, this);
        }

        public void Connect(TractorBeamConnection tractorBeamConnection)
        {
            // The connection can only connect if its not already connnected
            if (this.TractorBeamConnection != null)
                throw new Exception("Unable to connect! Already bound to another connection.");
            else if (tractorBeamConnection.State != ConnectionState.Connecting)
                throw new Exception("Unable to connect! Connection state is not set to Connecting.");
            else if (tractorBeamConnection.Target != this)
                throw new Exception("Unable to connect! Incoming connection does not reference current ITractorableEntity.");

            // Save the incoming connection
            this.TractorBeamConnection = tractorBeamConnection;

            this.SetEnabled(true);
            this.Body.SleepingAllowed = false;

            this.OnTractorBeamConnected?.Invoke(this, this);
        }
    }
}

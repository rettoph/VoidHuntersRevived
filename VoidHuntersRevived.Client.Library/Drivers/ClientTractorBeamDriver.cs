using FarseerPhysics.Dynamics;
using Guppy.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientTractorBeamDriver : Driver
    {
        #region Private Fields
        private TractorBeam _tractorBeam;
        #endregion

        #region Constructors
        public ClientTractorBeamDriver(TractorBeam parent, IServiceProvider provider) : base(parent, provider)
        {
            _tractorBeam = parent;
        }
        #endregion
    }
}

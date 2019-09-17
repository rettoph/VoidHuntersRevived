using GalacticFighters.Library.Entities;
using Guppy;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities
{
    public sealed class ClientTractorBeamDriver : Driver<TractorBeam>
    {
        #region Constructor
        public ClientTractorBeamDriver(TractorBeam driven) : base(driven)
        {
        }
        #endregion
    }
}

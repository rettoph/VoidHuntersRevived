using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Connections;

namespace VoidHuntersRevived.Library.Entities.MetaData
{
    public class ShipPartData
    {
        public readonly MaleConnection MaleConnection;

        public ShipPartData(MaleConnection maleConnection)
        {
            this.MaleConnection = maleConnection;
        }
    }
}

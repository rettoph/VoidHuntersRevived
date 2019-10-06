using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains import export funxtions,
    /// used for ship saving
    /// </summary>
    public partial class ShipPart
    {
        #region Export
        protected internal virtual void Export(BinaryWriter writer)
        {

        }
        #endregion

        #region Import
        protected internal virtual void Import(BinaryReader reader)
        {

        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains methods specific to sharing,
    /// ie import and export.
    /// 
    /// The import method will create brand new entities
    /// as needed, acting as a factory for the entire chain
    /// </summary>
    public partial class ShipPart
    {
        #region Export
        protected internal virtual void Export(BinaryWriter writer)
        {
            // 
        }
        #endregion

        #region Import
        protected internal virtual void Import(BinaryReader reader)
        {
            // 
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VoidHuntersRevived.Library.Contexts
{
    public abstract class WeaponContext : ShipPartContext
    {
        #region Public Properties
        /// <summary>
        /// The amount the described weapon can swivel (in radians)
        /// </summary>
        public Single SwivelRange { get; set; } = 2;
        #endregion

        #region Constructors
        protected WeaponContext(string name) : base(name)
        {
        }
        #endregion

        #region Serialization
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            this.SwivelRange = reader.ReadSingle();
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(this.SwivelRange);
        }
        #endregion
    }
}

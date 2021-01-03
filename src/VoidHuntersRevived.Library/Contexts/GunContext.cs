using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VoidHuntersRevived.Library.Contexts
{
    public class GunContext : WeaponContext
    {
        #region Public Properties 
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => "entity:ship-part:weapon:gun";

        /// <summary>
        /// Defines how much damage a bullet fired from this
        /// gun will do
        /// </summary>
        public Single BulletDamage { get; set; } = 10;
        #endregion

        #region Constructor
        public GunContext(string name) : base(name)
        {
        }
        #endregion

        #region Serialization Methods
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            this.BulletDamage = reader.ReadSingle();
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(this.BulletDamage);
        }
        #endregion
    }
}

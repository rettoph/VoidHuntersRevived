using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Gun", "Basic gun")]
    public class GunContext : WeaponContext
    {
        #region Public Properties 
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.Gun;

        /// <summary>
        /// Defines how much damage a bullet fired from this
        /// gun will do
        /// </summary>
        [ShipPartContextProperty("Bullet Damage", "The amount of damage applied by bullets fired.", ShipPartContextPropertyType.Single)]
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

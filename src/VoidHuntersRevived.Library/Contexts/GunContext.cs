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
        #region Enums
        private enum GunContextProperty
        {
            Start = 0,
            End = 1,
            BulletDamage = 2,
            BulletSpeed = 3
        }
        #endregion

        #region Public Properties 
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.Gun;

        /// <summary>
        /// Defines how much damage a bullet fired from this
        /// gun will do
        /// </summary>
        [ShipPartContextProperty("Bullet Damage", "The amount of damage applied by bullets fired.", ShipPartContextPropertyType.Single)]
        public Single BulletDamage { get; set; } = 10f;

        /// <summary>
        /// The speed at which a bullet is fired from the described gun.
        /// </summary>
        [ShipPartContextProperty("Bullet Spped", "The speed at which a bullet is fired from the described gun.", ShipPartContextPropertyType.Single)]
        public Single BulletSpeed { get; set; } = 15f;
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

            GunContextProperty propertyType = (GunContextProperty)reader.ReadByte();
            if (propertyType == GunContextProperty.Start)
            {
                while (propertyType != GunContextProperty.End)
                {
                    propertyType = (GunContextProperty)reader.ReadByte();
                    switch (propertyType)
                    {
                        case GunContextProperty.BulletDamage:
                            this.BulletDamage = reader.ReadSingle();
                            break;
                        case GunContextProperty.BulletSpeed:
                            this.BulletSpeed = reader.ReadSingle();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)GunContextProperty.Start);

            writer.Write((Byte)GunContextProperty.BulletDamage);
            writer.Write(this.BulletDamage);

            writer.Write((Byte)GunContextProperty.BulletSpeed);
            writer.Write(this.BulletSpeed);

            writer.Write((Byte)GunContextProperty.End);
        }
        #endregion
    }
}

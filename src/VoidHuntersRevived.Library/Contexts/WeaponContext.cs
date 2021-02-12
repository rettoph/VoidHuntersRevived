using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    public abstract class WeaponContext : ShipPartContext
    {
        #region Enums
        private enum WeaponContextProperty
        {
            Start = 0,
            End = 1,
            SwivelRange = 2,
            Recoil = 3,
            FireRate = 4,
            MaximumAmmunitionAge = 5,
            MaximumOffsetFireRange = 6
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The amount the described weapon can swivel (in radians)
        /// </summary>
        [ShipPartContextProperty("Swivel Range", "The range (in degrees) this weapon can swivel.", ShipPartContextPropertyType.Radian)]
        public Single SwivelRange { get; set; } = MathHelper.ToRadians(120);

        /// <summary>
        /// The amount (in farseer units) the gun should recoil when fired.
        /// </summary>
        [ShipPartContextProperty("Recoil", "The amount (in farseer units) the gun should recoil when fired.", ShipPartContextPropertyType.Single)]
        public Single Recoil { get; set; } = 0.3f;

        /// <summary>
        /// The amount (in farseer units) the gun should recoil when fired.
        /// </summary>
        [ShipPartContextProperty("Fire Rate", "The speed at which the weapon is fired in milliseconds.", ShipPartContextPropertyType.Single)]
        public Single FireRate { get; set; } = 400;

        /// <summary>
        /// The maximum age (in milliseconds) ammunition may live before being auto disposed.
        /// </summary>
        [ShipPartContextProperty("Maximum Ammunition Age", "The maximum age (in seconds) ammunition may live before being auto disposed.", ShipPartContextPropertyType.Single)]
        public Single MaximumAmmunitionAge { get; set; } = 3f;

        /// <summary>
        /// The amount the weapon can be offset but still fire (in degrees).
        /// </summary>
        [ShipPartContextProperty("MaximumOffsetFireRange", "The amount the weapon can be offset but still fire (in degrees).", ShipPartContextPropertyType.Radian)]
        public Single MaximumOffsetFireRange { get; set; } = MathHelper.ToDegrees(15);
        #endregion

        #region Constructors
        protected WeaponContext(string name) : base(name)
        {
            this.DefaultColor = Color.Red;
        }
        #endregion

        #region Serialization
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            WeaponContextProperty propertyType = (WeaponContextProperty)reader.ReadByte();
            if (propertyType == WeaponContextProperty.Start)
            {
                while (propertyType != WeaponContextProperty.End)
                {
                    propertyType = (WeaponContextProperty)reader.ReadByte();
                    switch (propertyType)
                    {
                        case WeaponContextProperty.SwivelRange:
                            this.SwivelRange = reader.ReadSingle();
                            break;
                        case WeaponContextProperty.Recoil:
                            this.Recoil = reader.ReadSingle();
                            break;
                        case WeaponContextProperty.FireRate:
                            this.FireRate = reader.ReadSingle();
                            break;
                        case WeaponContextProperty.MaximumAmmunitionAge:
                            this.MaximumAmmunitionAge = reader.ReadSingle();
                            break;
                        case WeaponContextProperty.MaximumOffsetFireRange:
                            this.MaximumOffsetFireRange = reader.ReadSingle();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)WeaponContextProperty.Start);

            writer.Write((Byte)WeaponContextProperty.SwivelRange);
            writer.Write(this.SwivelRange);

            writer.Write((Byte)WeaponContextProperty.Recoil);
            writer.Write(this.Recoil);

            writer.Write((Byte)WeaponContextProperty.FireRate);
            writer.Write(this.FireRate);

            writer.Write((Byte)WeaponContextProperty.MaximumAmmunitionAge);
            writer.Write(this.MaximumAmmunitionAge);

            writer.Write((Byte)WeaponContextProperty.MaximumOffsetFireRange);
            writer.Write(this.MaximumOffsetFireRange);

            writer.Write((Byte)WeaponContextProperty.End);
        }
        #endregion
    }
}

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
            SwivelRange = 2
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The amount the described weapon can swivel (in radians)
        /// </summary>
        [ShipPartContextProperty("Swivel Range", "The range (in degrees) this weapon can swivel.", ShipPartContextPropertyType.Radian)]
        public Single SwivelRange { get; set; } = MathHelper.ToRadians(120);
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

            writer.Write((Byte)WeaponContextProperty.End);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Laser", "Basic laser")]
    public class LaserContext : WeaponContext
    {
        #region Enums
        private enum LaserContextProperty
        {
            Start = 0,
            End = 1,
            DamagePerSecond = 2
        }
        #endregion

        #region Public Properties
        public override string ShipPartServiceConfiguration => VHR.Entities.Laser;

        /// <summary>
        /// Defines how much damage a bullet fired from this
        /// gun will do
        /// </summary>
        [ShipPartContextProperty("Damage Per Second", "The amount of damage applied by the laser beam per second.", ShipPartContextPropertyType.Single)]
        public Single DamagePerSecond { get; set; } = 10f;
        #endregion

        #region Constrcutor
        public LaserContext(string name) : base(name)
        {
        }
        #endregion

        #region Serialization Methods
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            LaserContextProperty propertyType = (LaserContextProperty)reader.ReadByte();
            if (propertyType == LaserContextProperty.Start)
            {
                while (propertyType != LaserContextProperty.End)
                {
                    propertyType = (LaserContextProperty)reader.ReadByte();
                    switch (propertyType)
                    {
                        case LaserContextProperty.DamagePerSecond:
                            this.DamagePerSecond = reader.ReadSingle();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)LaserContextProperty.Start);

            writer.Write((Byte)LaserContextProperty.DamagePerSecond);
            writer.Write(this.DamagePerSecond);

            writer.Write((Byte)LaserContextProperty.End);
        }
        #endregion
    }
}

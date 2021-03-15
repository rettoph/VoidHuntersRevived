using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Shield Generator", "Capable of generating force-field like shields.")]
    public class ShieldGeneratorContext : SpellPartContext
    {
        #region Enums
        private enum ShieldGeneratorContextProperty
        {
            Start = 0,
            End = 1,
            Radius = 2,
            Range = 3
        }
        #endregion

        #region Public Properties
        public override string ShipPartServiceConfiguration => VHR.Entities.ShieldGenerator;

        [ShipPartContextProperty("Radius", "The amount of damage applied by the laser beam per second.", ShipPartContextPropertyType.Single)]
        public Single Radius { get; set; } = 10;

        [ShipPartContextProperty("Range", "The amount of damage applied by the laser beam per second.", ShipPartContextPropertyType.Radian)]
        public Single Range { get; set; } = MathHelper.ToRadians(90);
        #endregion

        #region Constructors 
        public ShieldGeneratorContext(string name) : base(name)
        {
        }
        #endregion

        #region Serialization Methods
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            ShieldGeneratorContextProperty propertyType = (ShieldGeneratorContextProperty)reader.ReadByte();
            if (propertyType == ShieldGeneratorContextProperty.Start)
            {
                while (propertyType != ShieldGeneratorContextProperty.End)
                {
                    propertyType = (ShieldGeneratorContextProperty)reader.ReadByte();
                    switch (propertyType)
                    {
                        case ShieldGeneratorContextProperty.Radius:
                            this.Radius = reader.ReadSingle();
                            break;
                        case ShieldGeneratorContextProperty.Range:
                            this.Range = reader.ReadSingle();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)ShieldGeneratorContextProperty.Start);

            writer.Write((Byte)ShieldGeneratorContextProperty.Radius);
            writer.Write(this.Radius);

            writer.Write((Byte)ShieldGeneratorContextProperty.Range);
            writer.Write(this.Range);

            writer.Write((Byte)ShieldGeneratorContextProperty.End);
        }
        #endregion
    }
}

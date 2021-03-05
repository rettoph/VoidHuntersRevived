using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Power Cell", "A special piece designed to add energy capacity & increase energy charge speed.")]
    public class PowerCellContext : RigidShipPartContext
    {
        #region Enums
        private enum PowerCellContextProperty
        {
            Start = 0,
            End = 1,
            EnergyCapacity = 2,
            EnergyChargePerSecond = 3
        }
        #endregion

        #region Public Properties
        public override string ShipPartServiceConfiguration => VHR.Entities.PowerCell;

        /// <summary>
        /// The amount of energy this power cell gives.
        /// </summary>
        [ShipPartContextProperty("Energy Capacity", "The amount of energy this power cell gives.", ShipPartContextPropertyType.Single)]
        public Single EnergyCapacity { get; set; } = 25f;

        /// <summary>
        /// The energy charge boost given by this power cell.
        /// </summary>
        [ShipPartContextProperty("Energy Charge Per Second", "The energy charge boost given by this power cell.", ShipPartContextPropertyType.Single)]
        public Single EnergyChargePerSecond { get; set; } = 25f;
        #endregion

        #region Constructor
        public PowerCellContext(string name) : base(name)
        {
        }
        #endregion

        #region Serialization Methods
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            PowerCellContextProperty propertyType = (PowerCellContextProperty)reader.ReadByte();
            if (propertyType == PowerCellContextProperty.Start)
            {
                while (propertyType != PowerCellContextProperty.End)
                {
                    propertyType = (PowerCellContextProperty)reader.ReadByte();
                    switch (propertyType)
                    {
                        case PowerCellContextProperty.EnergyCapacity:
                            this.EnergyCapacity = reader.ReadSingle();
                            break;
                        case PowerCellContextProperty.EnergyChargePerSecond:
                            this.EnergyChargePerSecond = reader.ReadSingle();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)PowerCellContextProperty.Start);

            writer.Write((Byte)PowerCellContextProperty.EnergyCapacity);
            writer.Write(this.EnergyCapacity);

            writer.Write((Byte)PowerCellContextProperty.EnergyChargePerSecond);
            writer.Write(this.EnergyChargePerSecond);

            writer.Write((Byte)PowerCellContextProperty.End);
        }
        #endregion
    }
}

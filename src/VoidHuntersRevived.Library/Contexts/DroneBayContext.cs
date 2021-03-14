using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("DroneBay", "A special piece capable of launching another ship.")]
    public class DroneBayContext : SpellPartContext
    {
        #region Enums
        private enum DroneBayContextProperty
        {
            Start = 0,
            End = 1,
            DroneMaxAge = 2,
            DroneType = 3
        }
        #endregion

        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.DroneBay;
        #endregion

        #region Public Properties
        /// <summary>
        /// Defines how often this bay is capable of launching its drones.
        /// </summary>
        [ShipPartContextProperty("Drone Max Age", "Defines how long a drone launched from this bay will survive before self destructing.", ShipPartContextPropertyType.Single)]
        public Single DroneMaxAge { get; set; } = 45f;

        /// <summary>
        /// The type of drone launched form this bay.
        /// </summary>
        [ShipPartContextProperty("Drone Type", "The type of drone launched form this bay.", ShipPartContextPropertyType.String)]
        public String DroneType { get; set; } = "fighter";
        #endregion

        #region Constructor
        public DroneBayContext(String name) : base(name)
        {
            this.DefaultColor = Color.Pink;
        }
        #endregion

        #region Serialization Methods
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            DroneBayContextProperty propertyType = (DroneBayContextProperty)reader.ReadByte();
            if (propertyType == DroneBayContextProperty.Start)
            {
                while (propertyType != DroneBayContextProperty.End)
                {
                    propertyType = (DroneBayContextProperty)reader.ReadByte();
                    switch (propertyType)
                    {
                        case DroneBayContextProperty.DroneMaxAge:
                            this.DroneMaxAge = reader.ReadSingle();
                            break;
                        case DroneBayContextProperty.DroneType:
                            this.DroneType = reader.ReadString();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)DroneBayContextProperty.Start);

            writer.Write((Byte)DroneBayContextProperty.DroneMaxAge);
            writer.Write(this.DroneMaxAge);

            writer.Write((Byte)DroneBayContextProperty.DroneType);
            writer.Write(this.DroneType);

            writer.Write((Byte)DroneBayContextProperty.End);
        }
        #endregion
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Extensions.System.IO;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Thruster", "Represents a basic thruster capable of applying force at a point.")]
    public class ThrusterContext : ShipPartContext
    {
        #region Enums
        private enum ThrusterContextProperty
        {
            Start = 0,
            End = 1,
            MaxImpulse = 2,
            ImpulseAcceleration = 3
        }
        #endregion

        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.Thruster;
        #endregion

        #region Public Properties
        /// <summary>
        /// The maximum force per second 
        /// achievable by the current thruster.
        /// </summary>
        [ShipPartContextProperty("Maximum Impulse", "The maximum impulse achieved by this thruster.", ShipPartContextPropertyType.Vector2)]
        public Vector2 MaxImpulse { get; set; } = Vector2.UnitX * 10f;

        /// <summary>
        /// The impulse acceleration per second.
        /// </summary>
        [ShipPartContextProperty("Impulse Acceleration", "The impulse acceleration per second.", ShipPartContextPropertyType.Single)]
        public Single ImpulseAcceleration { get; set; } = 5f;
        #endregion

        #region Constructors
        public ThrusterContext(String name) : base(name)
        {
            this.DefaultColor = Color.LimeGreen;
        }
        #endregion

        #region Serialization Methods
        protected override void Read(BinaryReader reader)
        {
            base.Read(reader);

            ThrusterContextProperty propertyType = (ThrusterContextProperty)reader.ReadByte();
            if (propertyType == ThrusterContextProperty.Start)
            {
                while (propertyType != ThrusterContextProperty.End)
                {
                    propertyType = (ThrusterContextProperty)reader.ReadByte();
                    switch (propertyType)
                    {
                        case ThrusterContextProperty.MaxImpulse:
                            this.MaxImpulse = reader.ReadVector2();
                            break;
                        case ThrusterContextProperty.ImpulseAcceleration:
                            this.ImpulseAcceleration = reader.ReadSingle();
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((Byte)ThrusterContextProperty.Start);

            writer.Write((Byte)ThrusterContextProperty.MaxImpulse);
            writer.Write(this.MaxImpulse);

            writer.Write((Byte)ThrusterContextProperty.ImpulseAcceleration);
            writer.Write(this.ImpulseAcceleration);

            writer.Write((Byte)ThrusterContextProperty.End);
        }
        #endregion
    }
}

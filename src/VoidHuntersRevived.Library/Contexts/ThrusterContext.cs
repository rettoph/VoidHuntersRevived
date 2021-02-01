using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Thruster", "Represents a basic thruster capable of applying force at a point.")]
    public class ThrusterContext : RigidShipPartContext
    {
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
        public virtual Vector2 MaxImpulse { get; set; } = Vector2.UnitX * 10f;
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

            this.MaxImpulse = new Vector2(reader.ReadSingle(), reader.Read());
        }

        protected override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(this.MaxImpulse.X);
            writer.Write(this.MaxImpulse.Y);
        }
        #endregion
    }
}

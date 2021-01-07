using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Lists;
using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Contexts
{
    /// <summary>
    /// The base ship part context class, used to define
    /// custom <see cref="ShipPart"/> configuration values.
    /// 
    /// In implementing type must be used in order to utilize this
    /// class.
    /// </summary>
    public abstract class ShipPartContext
    {
        #region Public Properties
        /// <summary>
        /// The cross platform unique key for this context.
        /// </summary>
        public UInt32 Id => this.Name.xxHash();

        /// <summary>
        /// A unique name linked to this context.
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// A list of all raw Shape data contained within the ShipPart.
        /// </summary>
        public ShipPartShapesContext Shapes { get; private set; }

        /// <summary>
        /// The default <see cref="Color"/> to render the 
        /// <see cref="ShipPart"/> when not attached to any
        /// <see cref="Ship"/> or <see cref="Chain"/>
        /// </summary>
        public Color DefaultColor { get; set; } = Color.Orange;

        /// <summary>
        /// When true, the rendered color will copy that of the parent.
        /// </summary>
        public Boolean InheritColor { get; set; } = true;

        /// <summary>
        /// The maximum <see cref="ShipPart.Health"/> value
        /// for the defined <see cref="ShipPart"/>.
        /// </summary>
        public Single MaxHealth { get; set; } = 100f;

        /// <summary>
        /// The name of the service configuration to be used
        /// when creating a new instance of the defined <see cref="ShipPart"/>
        /// </summary>
        public abstract String ShipPartServiceConfiguration { get; }

        /// <summary>
        /// A multiplier applied to all <see cref="ShipPartShapeContext.Density"/> 
        /// values when generating a <see cref="Fixture"/>.
        /// </summary>
        public Single DensityMultiplier { get; set; } = 1f;
        #endregion

        #region Constructor
        public ShipPartContext(String name)
        {
            this.Name = name;
            this.Shapes = new ShipPartShapesContext();
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Attempt to read the internal data directly 
        /// from the recieved <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"></param>
        public Boolean TryRead(BinaryReader reader)
        {
            try
            {
                this.Read(reader);
                return true;
            }
            catch(Exception e)
            {
                // There was some sort of error deserializing the stream data...
                return false;
            }
        }

        /// <summary>
        /// Attempt to write the self contained data to 
        /// the recieved <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer"></param>
        public void TryWrite(BinaryWriter writer)
        {
            var position = writer.BaseStream.Position;

            try
            {
                // Append the context type for future reflection & deserialization.
                writer.Write(this.GetType().FullName.xxHash());
                writer.Write(this.Name);

                this.Write(writer);
            }
            catch (Exception e)
            {
                // There was some sort of error serializing the stream data...
                writer.BaseStream.Position = position;
            }
        }

        /// <summary>
        /// Save the current context data to a target ile.
        /// </summary>
        /// <param name="location"></param>
        public void Export(String location)
        {
            using (FileStream file = File.Open(location, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new BinaryWriter(file))
                this.TryWrite(writer);
        }

        /// <summary>
        /// Create a new instance of the context's defined ShipPart.
        /// </summary>
        /// <returns></returns>
        public virtual ShipPart Create(EntityList entities, Action<ShipPart, ServiceProvider, ServiceConfiguration> setup = null)
            => entities.Create<ShipPart>(this.ShipPartServiceConfiguration, (sp, p, c) =>
            {
                sp.SetContext(this);

                setup?.Invoke(sp, p, c);
            });
        #endregion

        #region Internal Methods
        /// <summary>
        /// Read the internal data directly 
        /// from the recieved <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void Read(BinaryReader reader)
        {
            this.MaxHealth = reader.ReadSingle();
            this.DefaultColor = new Color(packedValue: reader.ReadUInt32());
            this.InheritColor = reader.ReadBoolean();
            this.Shapes.Read(reader);
        }

        /// <summary>
        /// Write the self contained data to 
        /// the recieved <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void Write(BinaryWriter writer)
        {
            writer.Write(this.MaxHealth);
            writer.Write(this.DefaultColor.PackedValue);
            writer.Write(this.InheritColor);
            this.Shapes.Write(writer);
        }
        #endregion
    }
}

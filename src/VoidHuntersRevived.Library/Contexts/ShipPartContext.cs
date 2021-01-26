using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Lists;
using Guppy.Utilities.Primitives;
using K4os.Hash.xxHash;
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
using VoidHuntersRevived.Library.Extensions.System.IO;
using VoidHuntersRevived.Library.Attributes;

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
        #region Abstract Properties
        /// <summary>
        /// The name of the service configuration to be used
        /// when creating a new instance of the defined <see cref="ShipPart"/>.
        /// </summary>
        public abstract String ShipPartServiceConfiguration { get; }
        #endregion

        #region Public Properties
        /// <summary>
        /// The cross platform unique key for this context.
        /// </summary>
        public UInt32 Id => this.Name.xxHash();

        /// <summary>
        /// A unique name linked to this context.
        /// </summary>
        [ShipPartContextProperty("Name", "A unique identifier for your part", ShipPartContextPropertyType.String)]
        public String Name { get; set; }

        /// <summary>
        /// The default <see cref="Color"/> to render the 
        /// <see cref="ShipPart"/> when not attached to any
        /// <see cref="Ship"/> or <see cref="Chain"/>
        /// </summary>
        [ShipPartContextProperty("Default Color", "The part's default color when free floating in space.", ShipPartContextPropertyType.Color)]
        public Color DefaultColor { get; set; } = Color.Orange;

        /// <summary>
        /// When true, the rendered color will copy that of the parent.
        /// </summary>
        [ShipPartContextProperty("Inherit Color", "Determins if the current part should inherit its color or not.", ShipPartContextPropertyType.Boolean)]
        public Boolean InheritColor { get; set; } = true;

        /// <summary>
        /// The maximum <see cref="ShipPart.Health"/> value
        /// for the defined <see cref="ShipPart"/>.
        /// </summary>
        [ShipPartContextProperty("Max Health", "The maximum amount of health the current part contains.", ShipPartContextPropertyType.Single)]
        public Single MaxHealth { get; set; } = 100f;

        /// <summary>
        /// The density of the current defined ShipPart
        /// </summary>
        [ShipPartContextProperty("Density", "The current part's density.", ShipPartContextPropertyType.Single)]
        public Single Density { get; set; } = 1f;

        /// <summary>
        /// A custom defined centeroid for the current <see cref="Shapes"/>.
        /// </summary>
        [ShipPartContextProperty("Centeroid", "A custom defined centeroid for the part.", ShipPartContextPropertyType.Vector2)]
        public Vector2 Centeroid { get; set; }

        /// <summary>
        /// The *lastmode* defined male connection node.
        /// Note, male connection nodes can be manually overwritten
        /// in newer shapes. This is bad practice, but for now it
        /// works. Just make sure you define the male connection
        /// node in the last shape to be sure it never gets overwritten.
        /// </summary>
        public ConnectionNodeContext MaleConnectionNode { get; set; } = new ConnectionNodeContext();

        /// <summary>
        /// A list of all contained female connection nodes.
        /// </summary>
        public ConnectionNodeContext[] FemaleConnectionNodes { get; set; } = new ConnectionNodeContext[0];

        /// <summary>
        /// All distinct shapes within the current part, multiple may be used in combination
        /// to create concave shapes.
        /// </summary>
        public Vertices[] InnerShapes { get; set; } = new Vertices[0];

        /// <summary>
        /// A ccollection of traced outline used primarily to render
        /// the shape. This can be used for adding detail or cleaning
        /// the default concave to convex outline conversion.
        /// </summary>
        public Vertices[] OuterHulls { get; set; } = new Vertices[0];
        #endregion

        #region Constructor
        public ShipPartContext(String name)
        {
            this.Name = name;
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
            this.Read(reader);
            return true;
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

            this.Centeroid = reader.ReadVector2();
            this.MaleConnectionNode = reader.ReadConnectionNodeContext();

            var nodeBuffer = new List<ConnectionNodeContext>();
            var nodeCount = reader.ReadInt32();
            for (var i = 0; i < nodeCount; i++)
                nodeBuffer.Add(reader.ReadConnectionNodeContext());
            this.FemaleConnectionNodes = nodeBuffer.ToArray();
            nodeBuffer.Clear();


            var vertBuffer = new List<Vertices>();
            var shapeCount = reader.ReadInt32();
            for (var i = 0; i < shapeCount; i++)
                vertBuffer.Add(reader.ReadVertices());
            this.InnerShapes = vertBuffer.ToArray();
            vertBuffer.Clear();

            var hullCount = reader.ReadInt32();
            for (var i = 0; i < hullCount; i++)
                vertBuffer.Add(reader.ReadVertices());
            this.OuterHulls = vertBuffer.ToArray();
            vertBuffer.Clear();
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

            writer.Write(this.Centeroid);
            writer.Write(this.MaleConnectionNode);

            writer.Write(this.FemaleConnectionNodes.Length);
            foreach (ConnectionNodeContext female in this.FemaleConnectionNodes)
                writer.Write(female);

            writer.Write(this.InnerShapes.Length);
            foreach (Vertices shape in this.InnerShapes)
                writer.Write(shape);

            writer.Write(this.OuterHulls.Length);
            foreach (Vertices outerHull in this.OuterHulls)
                writer.Write(outerHull);
        }
        #endregion
    }
}

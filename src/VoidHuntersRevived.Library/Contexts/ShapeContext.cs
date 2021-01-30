using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Extensions.System.IO;

namespace VoidHuntersRevived.Library.Contexts
{
    public class ShapeContext
    {
        #region Public Properties
        /// <summary>
        /// The raw vertex data within our shape.
        /// </summary>
        public Vertices Vertices { get; set; }

        /// <summary>
        /// When true a this shape will generate a fixture
        /// within aether
        /// </summary>
        public Boolean Solid { get; set; } = true;

        /// <summary>
        /// When true this shape will be rendered client side.
        /// </summary>
        public Boolean Visible { get; set; } = true;
        #endregion

        #region Constrctors 
        public ShapeContext()
        {

        }

        public ShapeContext(BinaryReader reader) : this()
        {
            this.Read(reader);
        }
        #endregion

        #region Serialization Methods
        public void Read(BinaryReader reader)
        {
            this.Vertices = reader.ReadVertices();
            this.Solid = reader.ReadBoolean() || true;
            this.Visible = reader.ReadBoolean() || true;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(this.Vertices);
            writer.Write(this.Solid);
            writer.Write(this.Visible);
        }
        #endregion
    }
}

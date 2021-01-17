using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Library.Extensions.System.IO
{
    public static class BinaryWriterExtensions
    {
        #region Vector2 Methods
        public static void Write(this BinaryWriter writer, Vector2 vector2)
        {
            writer.Write(vector2.X);
            writer.Write(vector2.Y);
        }
        #endregion

        #region Vertices Methods
        public static void Write(this BinaryWriter writer, Vertices vertices)
        {
            writer.Write(vertices.Count);
            foreach (Vector2 vector2 in vertices)
                writer.Write(vector2);
        }
        #endregion

        #region Color Methods
        public static void Write(this BinaryWriter writer, Color color)
        {
            writer.Write(color.PackedValue);
        }
        #endregion

        #region ConnectionNodeContext Methods
        public static void Write(this BinaryWriter writer, ConnectionNodeContext node)
        {
            writer.Write(node.Position);
            writer.Write(node.Rotation);
        }
        #endregion
    }
}

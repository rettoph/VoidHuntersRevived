using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Library.Extensions.System.IO
{
    public static class BinaryReaderExtensions
    {
        #region Vector2 Methods
        public static Vector2 ReadVector2(this BinaryReader reader)
            => new Vector2(reader.ReadSingle(), reader.ReadSingle());
        #endregion

        #region Vertices Methods
        public static Vertices ReadVertices(this BinaryReader reader)
            => new Vertices(ReadVector2IEnumerable(reader));
        private static IEnumerable<Vector2> ReadVector2IEnumerable(BinaryReader reader)
        {
            var vertCount = reader.ReadInt32();
            for (var i = 0; i < vertCount; i++)
                yield return reader.ReadVector2();
        }
        #endregion

        #region Color Methods
        public static Color ReadColor(this BinaryReader reader)
            => new Color(reader.ReadUInt32());
        #endregion

        #region ConnectionNodeContext Methods
        public static ConnectionNodeContext ReadConnectionNodeContext(this BinaryReader reader)
            => new ConnectionNodeContext()
            {
                Position = reader.ReadVector2(),
                Rotation = reader.ReadSingle()
            };
        #endregion
    }
}

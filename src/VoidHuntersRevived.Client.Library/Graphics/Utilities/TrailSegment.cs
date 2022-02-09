using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library.Graphics.Effects;
using VoidHuntersRevived.Client.Library.Graphics.Vertices;

namespace VoidHuntersRevived.Client.Library.Graphics.Utilities
{
    internal class TrailSegment
    {
        public Double CreatedTimestamp;
        public Double MaxAge;

        public VertexTrail Port;
        public VertexTrail Starboard;

        public TrailSegment Next;

        internal void Draw(GameTime gameTime, PrimitiveBatch<VertexTrail, TrailEffect> primitiveBatch)
        {
            if(this.Next is null)
            {
                return;
            }

            primitiveBatch.DrawTriangle(ref this.Starboard, ref this.Port, ref this.Next.Port);
            primitiveBatch.DrawTriangle(ref this.Next.Starboard, ref this.Starboard, ref this.Next.Port);

            this.Next.Draw(gameTime, primitiveBatch);
        }
    }
}

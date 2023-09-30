using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct TraceVertices : IDisposable
    {
        public NativeDynamicArrayCast<TraceVertex> Items;

        public void Dispose()
        {
            this.Items.Dispose();
        }
    }
}

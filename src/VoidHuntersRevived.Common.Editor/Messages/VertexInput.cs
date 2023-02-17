using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Editor.Messages
{
    public class VertexInput : Message<VertexInput>
    {
        public enum Actions
        { 
            Add,
            Remove
        };

        public required Actions Action { get; init; }
        public Vector2? Value { get; }
    }
}

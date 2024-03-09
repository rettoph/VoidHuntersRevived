using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Client.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexStaticVisible : IVertexType
    {
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        private const uint IsTraceFlag = 0x00000001;
        private const uint IsOuterFlag = 0x00000002;
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Single, VertexElementUsage.BlendIndices, 0)
        );

        [FieldOffset(0)]
        public Vector2 Position;

        [FieldOffset(8)]
        private uint _flags;

        public bool Trace
        {
            set
            {
                if (value)
                {
                    _flags |= IsTraceFlag;
                }
                else
                {
                    _flags &= ~IsTraceFlag;
                }
            }
        }

        public bool Outer
        {
            set
            {
                if (value)
                {
                    _flags |= IsOuterFlag;
                }
                else
                {
                    _flags &= ~IsOuterFlag;
                }

                _flags |= IsTraceFlag;
            }
        }
    }
}

using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Domain.Client.Utilities
{
    internal sealed class VertexBufferManager<TVertex>
        where TVertex : struct, IVertexType
    {
        private GraphicsDevice _graphics;
        private int _size;

        public VertexBuffer VertexBuffer;
        public TVertex[] Data;
        public int Count;

        public VertexBufferManager(GraphicsDevice graphics, int initialSize)
        {
            _graphics = graphics;
            _size = initialSize;

            this.VertexBuffer = new DynamicVertexBuffer(_graphics, typeof(TVertex), _size, BufferUsage.WriteOnly);
            this.Data = new TVertex[_size];
        }

        public bool Resize(int size)
        {
            if (size <= _size)
            {
                return false;
            }

            _size = size;

            this.VertexBuffer?.Dispose();
            this.VertexBuffer = new DynamicVertexBuffer(_graphics, typeof(TVertex), _size, BufferUsage.WriteOnly);
            this.Data = new TVertex[_size];

            return true;
        }

        public void SetData(int count)
        {
            this.Count = count;

            this.VertexBuffer.SetData(this.Data, 0, count);
        }
    }
}

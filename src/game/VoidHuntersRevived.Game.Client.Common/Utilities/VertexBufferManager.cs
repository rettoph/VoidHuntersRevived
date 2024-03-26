using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Game.Client.Common.Utilities
{
    public class VertexBufferManager<TVertexInstance> : IDisposable
        where TVertexInstance : struct, IVertexType
    {
        private const int DefaultBufferSize = 256;

        private readonly GraphicsDevice _graphics;

        private readonly VertexBuffer _staticBuffer;
        private readonly IndexBuffer _indexBuffer;
        private VertexBuffer _instanceBuffer;
        private TVertexInstance[] _instanceVertices;
        private int _instanceCount = 0;
        private VertexBufferBinding[] _bindings;

        public VertexBufferBinding[] VertexBufferBindings => _bindings;
        public IndexBuffer IndexBuffer => _indexBuffer;
        public int StaticTriangleCount { get; }
        public int InstanceCount => _instanceCount;
        public int TriangleCount => this.StaticTriangleCount * this.InstanceCount;

        public VertexBufferManager(GraphicsDevice graphics, VertexBuffer staticBuffer, IndexBuffer indexBuffer)
        {
            _graphics = graphics;

            _instanceVertices = new TVertexInstance[DefaultBufferSize];
            _instanceBuffer = new DynamicVertexBuffer(_graphics, typeof(TVertexInstance), _instanceVertices.Length, BufferUsage.WriteOnly);

            _staticBuffer = staticBuffer;
            _indexBuffer = indexBuffer;
            _bindings = [
                new VertexBufferBinding(_staticBuffer, 0, 0),
                new VertexBufferBinding(_instanceBuffer, 0, 1)
            ];

            this.StaticTriangleCount = _indexBuffer.IndexCount / 3;
        }

        public void EnsureFit(int size)
        {
            if (_instanceVertices.Length >= size + _instanceCount)
            {
                return;
            }

            int capacity = _instanceVertices.Length;
            while (capacity < size + _instanceCount)
            {
                capacity *= 2;
            }

            Array.Resize(ref _instanceVertices, capacity);

            _instanceBuffer.Dispose();
            _instanceBuffer = new DynamicVertexBuffer(_graphics, typeof(TVertexInstance), _instanceVertices.Length, BufferUsage.WriteOnly);
            _bindings = [
                new VertexBufferBinding(_staticBuffer, 0, 0),
                new VertexBufferBinding(_instanceBuffer, 0, 1)
            ];
        }

        public void SetNextVertexUnsafe(TVertexInstance vertex)
        {
            _instanceVertices[_instanceCount++] = vertex;
        }

        public ref TVertexInstance GetNextVertexUnsafe()
        {
            return ref _instanceVertices[_instanceCount++];
        }

        public void SetNextVertex(TVertexInstance vertex)
        {
            this.EnsureFit(1);
            _instanceVertices[_instanceCount++] = vertex;
        }

        public ref TVertexInstance GetNextVertex()
        {
            this.EnsureFit(1);
            return ref _instanceVertices[_instanceCount++];
        }

        public void Flush()
        {
            _instanceBuffer.SetData(_instanceVertices, 0, _instanceCount);
        }

        public void Clear()
        {
            _instanceCount = 0;
        }

        public void Dispose()
        {
            _instanceBuffer.Dispose();
            _staticBuffer.Dispose();
        }
    }
}

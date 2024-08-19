using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Graphics.Common.Utilities
{
    public class VertexBuffer<TVertex> : IVertexBuffer<TVertex>
        where TVertex : unmanaged, IVertexType
    {
        private static int FilterId;
        private static FilterContextID FilterContextId = FilterContextID.GetNewContextID();

        private const int DefaultBufferSize = 256;

        private readonly GraphicsDevice _graphics;

        private readonly CombinedFilterID _combinedFilterId;
        private readonly VertexBuffer[] _staticBuffers;
        private readonly IndexBuffer[] _indexBuffers;
        private readonly PrimitiveType[] _primitiveTyes;
        private VertexBuffer _instanceBuffer;
        private TVertex[] _instanceVertices;
        private int _instanceCount = 0;
        private VertexBufferBinding[][] _bindings;

        public readonly int BufferCount;
        public VertexBufferBinding[][] VertexBufferBindings => _bindings;
        public IndexBuffer[] IndexBuffers => _indexBuffers;
        public PrimitiveType[] PrimitiveTypes => _primitiveTyes;
        public readonly int[] StaticPrimitiveCount;
        public int InstanceCount => _instanceCount;
        public Func<int, int> PrimitiveCount => (idx) => StaticPrimitiveCount[idx] * InstanceCount;

        public EntitiesDB EntitiesDb { get; set; } = null!;

        public VertexBuffer(
            GraphicsDevice graphics,
            VertexBuffer[] staticBuffers,
            IndexBuffer[] indexBuffers,
            PrimitiveType[] primitiveTypes)
        {
            if (staticBuffers.Length != indexBuffers.Length || staticBuffers.Length != primitiveTypes.Length)
            {
                throw new ArgumentException();
            }

            _graphics = graphics;
            _combinedFilterId = new CombinedFilterID(FilterId++, FilterContextId);
            _instanceVertices = new TVertex[DefaultBufferSize];
            _instanceBuffer = new DynamicVertexBuffer(_graphics, typeof(TVertex), _instanceVertices.Length, BufferUsage.WriteOnly);

            _staticBuffers = staticBuffers;
            _indexBuffers = indexBuffers;
            _primitiveTyes = primitiveTypes;
            _bindings = _staticBuffers.Select((x, idx) => new VertexBufferBinding[]
            {
                new VertexBufferBinding(_staticBuffers[idx], 0, 0),
                new VertexBufferBinding(_instanceBuffer, 0, 1)
            }).ToArray();

            StaticPrimitiveCount = _primitiveTyes.Select(x => x switch
            {
                PrimitiveType.LineList => 2,
                PrimitiveType.TriangleList => 3,
                _ => throw new NotImplementedException()
            }).Select((x, idx) => _indexBuffers[idx].IndexCount / x).ToArray();

            this.BufferCount = staticBuffers.Length;
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
            _instanceBuffer = new DynamicVertexBuffer(_graphics, typeof(TVertex), _instanceVertices.Length, BufferUsage.WriteOnly);
            _bindings = _staticBuffers.Select((x, idx) => new VertexBufferBinding[]
            {
                new VertexBufferBinding(_staticBuffers[idx], 0, 0),
                new VertexBufferBinding(_instanceBuffer, 0, 1)
            }).ToArray();
        }

        public void SetNextVertexUnsafe(TVertex vertex)
        {
            _instanceVertices[_instanceCount++] = vertex;
        }

        public ref TVertex GetNextVertexUnsafe()
        {
            return ref _instanceVertices[_instanceCount++];
        }

        public void SetNextVertex(TVertex vertex)
        {
            EnsureFit(1);
            _instanceVertices[_instanceCount++] = vertex;
        }

        public ref TVertex GetNextVertex()
        {
            EnsureFit(1);
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

        public void Draw(Effect effect)
        {
            for (int i = 0; i < this.BufferCount; i++)
            {
                _graphics.SetVertexBuffers(this.VertexBufferBindings[i]);
                _graphics.Indices = this.IndexBuffers[i];

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphics.DrawInstancedPrimitives(this.PrimitiveTypes[i], 0, 0, this.StaticPrimitiveCount[i], this.InstanceCount);
                }
            }
        }

        public void Dispose()
        {
            _instanceBuffer.Dispose();

            foreach (IndexBuffer indexBuffer in _indexBuffers)
            {
                indexBuffer.Dispose();
            }

            foreach (VertexBuffer staticBuffer in _staticBuffers)
            {
                staticBuffer.Dispose();
            }
        }

        public ref EntityFilterCollection GetFilter<TComponent>()
            where TComponent : unmanaged, IVertexType, IEntityComponent
        {
            return ref this.EntitiesDb.GetFilters().GetOrCreatePersistentFilter<TComponent>(_combinedFilterId);
        }
    }
}

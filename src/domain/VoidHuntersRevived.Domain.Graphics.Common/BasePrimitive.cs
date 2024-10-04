using Guppy.Core.Common;
using Guppy.Core.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using System.Diagnostics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Contexts;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    [DebuggerDisplay("Type = {Type.Name}, VertexType = {VertexType.Name}, SequenceGroup = {SequenceGroup}")]
    public abstract class BasePrimitive<TVertex> : IPrimitive<TVertex>, IQueryingEntitiesEngine
        where TVertex : unmanaged, IVertexType
    {
        private static int FilterId;
        private static readonly FilterContextID FilterContextId = FilterContextID.GetNewContextID();

        private const int DefaultBufferSize = 256;

        private readonly GraphicsDevice _graphics;

        private readonly CombinedFilterID _combinedFilterId;
        private readonly VertexBuffer[] _staticBuffers;
        private readonly IndexBuffer[] _indexBuffers;
        private readonly Effect _effect;
        private readonly PrimitiveType[] _primitiveTypes;
        private VertexBuffer _instanceBuffer;
        private TVertex[] _instanceVertices;
        private int _instanceCount = 0;
        private VertexBufferBinding[][] _bindings;

        public readonly int BufferCount;
        public VertexBufferBinding[][] VertexBufferBindings => _bindings;
        public IndexBuffer[] IndexBuffers => _indexBuffers;
        public PrimitiveType[] PrimitiveTypes => _primitiveTypes;
        public readonly int[] StaticPrimitiveCount;
        public int InstanceCount => _instanceCount;
        public Effect Effect => _effect;
        public Func<int, int> PrimitiveCount => (idx) => StaticPrimitiveCount[idx] * InstanceCount;

        public Key<IPrimitive> Type { get; }
        public PrimitiveSequenceGroupEnum SequenceGroup { get; }
        public Type VertexType => typeof(TVertex);

        public int Sequence { get; }
        public EntitiesDB entitiesDB { get; set; } = null!;

        SequenceGroup<PrimitiveSequenceGroupEnum> IRuntimeSequenceGroup<PrimitiveSequenceGroupEnum>.Value => SequenceGroup<PrimitiveSequenceGroupEnum>.GetByValue(this.SequenceGroup);

        public BasePrimitive(
            PrimitiveContext context,
            PrimitiveSequenceGroupEnum sequenceGroup,
            GraphicsDevice graphics,
            BufferContext staticBufferContext,
            Effect effect)
        {
            _graphics = graphics;
            _combinedFilterId = new CombinedFilterID(FilterId++, FilterContextId);
            _instanceVertices = new TVertex[DefaultBufferSize];
            _instanceBuffer = new DynamicVertexBuffer(_graphics, typeof(TVertex), _instanceVertices.Length, BufferUsage.WriteOnly);
            _effect = effect;

            _staticBuffers = staticBufferContext.BuildVertexBuffers(graphics);
            _indexBuffers = staticBufferContext.BuildIndexBuffers(graphics);
            _primitiveTypes = [.. staticBufferContext.GetTypes()];
            _bindings = _staticBuffers.Select((x, idx) => new VertexBufferBinding[]
            {
                new(_staticBuffers[idx], 0, 0),
                new(_instanceBuffer, 0, 1)
            }).ToArray();

            this.StaticPrimitiveCount = staticBufferContext.GetTypes().Select(x => x switch
            {
                PrimitiveType.LineList => 2,
                PrimitiveType.TriangleList => 3,
                _ => throw new NotImplementedException()
            }).Select((x, idx) => _indexBuffers[idx].IndexCount / x).ToArray();

            this.BufferCount = _staticBuffers.Length;
            this.Type = context.Type;
            this.SequenceGroup = sequenceGroup;
            this.Sequence = context.Sequence;
        }

        public void Ready()
        {
            //
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
                new(_staticBuffers[idx], 0, 0),
                new(_instanceBuffer, 0, 1)
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

        protected virtual bool Flush()
        {
            if (_instanceCount == 0)
            {
                return false;
            }

            _instanceBuffer.SetData(_instanceVertices, 0, _instanceCount);
            return true;
        }

        protected virtual void Clear()
        {
            _instanceCount = 0;
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (this.Flush() == false)
            {
                return;
            }

            _graphics.BlendState = BlendState.NonPremultiplied;

            for (int i = 0; i < this.BufferCount; i++)
            {
                _graphics.SetVertexBuffers(this.VertexBufferBindings[i]);
                _graphics.Indices = this.IndexBuffers[i];

                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphics.DrawInstancedPrimitives(this.PrimitiveTypes[i], 0, 0, this.StaticPrimitiveCount[i], this.InstanceCount);
                }
            }

            this.Clear();
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
            return ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<TComponent>(_combinedFilterId);
        }
    }
}

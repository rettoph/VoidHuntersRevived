using Guppy.Core.Common;
using Guppy.Core.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics
{
    public abstract class Primitive : IPrimitive
    {
        private static int FilterId;
        private static readonly FilterContextID FilterContextId = FilterContextID.GetNewContextID();

        public Type VertexType { get; }
        public int Sequence { get; }
        public VertexBuffer StaticVertexBuffer { get; }
        public IndexBuffer[] StaticIndexBuffers { get; }
        public PrimitiveTypeEnum[] BufferTypes { get; }
        public int[] StaticPrimitiveCount { get; }
        public GraphicsDevice Graphics { get; }
        public PrimitiveSequenceGroupEnum SequenceGroup { get; }
        public int InstanceCount { get; protected set; }
        public VertexBuffer InstanceVertexBuffer { get; protected set; }
        public VertexBufferBinding[][] VertexBufferBindings { get; protected set; }
        public CombinedFilterID CombinedFilterId { get; }

        internal Primitive(
            Type vertexType,
            int sequence,
            PrimitiveSequenceGroupEnum sequenceGroup,
            VertexBuffer staticVertexBuffer,
            IndexBuffer[] staticIndexBuffers,
            PrimitiveTypeEnum[] bufferTypes,
            GraphicsDevice graphics)
        {
            this.VertexType = vertexType;
            this.Sequence = sequence;
            this.SequenceGroup = sequenceGroup;
            this.StaticVertexBuffer = staticVertexBuffer;
            this.StaticIndexBuffers = staticIndexBuffers;
            this.BufferTypes = bufferTypes;
            this.Graphics = graphics;
            this.StaticPrimitiveCount = this.BufferTypes.Select(x => x switch
            {
                PrimitiveTypeEnum.LineList => 2,
                PrimitiveTypeEnum.TriangleList => 3,
                _ => throw new NotImplementedException()
            }).Select((x, idx) => this.StaticIndexBuffers[idx].IndexCount / x).ToArray();

            this.InstanceVertexBuffer = default!;
            this.VertexBufferBindings = [];

            this.CombinedFilterId = new CombinedFilterID(FilterId++, FilterContextId);
        }

        public abstract void Dispose();
    }

    public abstract class Primitive<TVertexInstance> : Primitive, IPrimitive<TVertexInstance>
        where TVertexInstance : unmanaged, IVertexType
    {
        private const int DefaultBufferSize = 256;

        private TVertexInstance[] _instanceVertices;

        public TVertexInstance[] InstanceVertices => _instanceVertices;

        internal Primitive(
            int sequence,
            PrimitiveSequenceGroupEnum sequenceGroup,
            VertexBuffer staticVertexBuffer,
            IndexBuffer[] staticIndexBuffers,
            PrimitiveTypeEnum[] bufferTypes,
            GraphicsDevice graphics) : base(typeof(TVertexInstance), sequence, sequenceGroup, staticVertexBuffer, staticIndexBuffers, bufferTypes, graphics)
        {
            _instanceVertices = new TVertexInstance[DefaultBufferSize];
            this.InstanceVertexBuffer = new DynamicVertexBuffer(this.Graphics, typeof(TVertexInstance), _instanceVertices.Length, BufferUsage.WriteOnly);
            this.VertexBufferBindings = this.StaticIndexBuffers.Select((x, idx) => new VertexBufferBinding[]
            {
                new(this.StaticVertexBuffer, 0, 0),
                new(this.InstanceVertexBuffer, 0, 1)
            }).ToArray();
        }

        public void EnsureFit(int size)
        {
            if (_instanceVertices.Length >= size + this.InstanceCount)
            {
                return;
            }

            int capacity = _instanceVertices.Length;
            while (capacity < size + this.InstanceCount)
            {
                capacity *= 2;
            }

            Array.Resize(ref _instanceVertices, capacity);

            this.InstanceVertexBuffer.Dispose();
            this.InstanceVertexBuffer = new DynamicVertexBuffer(this.Graphics, typeof(TVertexInstance), _instanceVertices.Length, BufferUsage.WriteOnly);
            this.VertexBufferBindings = this.StaticIndexBuffers.Select((x, idx) => new VertexBufferBinding[]
            {
                new(this.StaticVertexBuffer, 0, 0),
                new(this.InstanceVertexBuffer, 0, 1)
            }).ToArray();
        }

        public void SetNextVertexUnsafe(TVertexInstance vertex)
        {
            _instanceVertices[this.InstanceCount++] = vertex;
        }

        public ref TVertexInstance GetNextVertexUnsafe()
        {
            return ref _instanceVertices[this.InstanceCount++];
        }

        public void SetNextVertex(TVertexInstance vertex)
        {
            EnsureFit(1);
            _instanceVertices[this.InstanceCount++] = vertex;
        }

        public ref TVertexInstance GetNextVertex()
        {
            EnsureFit(1);
            return ref _instanceVertices[this.InstanceCount++];
        }

        public virtual bool Flush()
        {
            if (this.InstanceCount == 0)
            {
                return false;
            }

            this.InstanceVertexBuffer.SetData(_instanceVertices, 0, this.InstanceCount);
            return true;
        }

        public virtual void Clear()
        {
            this.InstanceCount = 0;
        }

        public abstract void Draw(GameTime gameTime);

        public override void Dispose()
        {
            this.InstanceVertexBuffer?.Dispose();
        }
    }

    public class Primitive<TVertexInstance, TVertexStatic, TEffect>(
            int sequence,
            PrimitiveSequenceGroupEnum sequenceGroup,
            VertexBuffer staticVertexBuffer,
            IndexBuffer[] staticIndexBuffers,
            PrimitiveTypeEnum[] bufferTypes,
            TEffect effect,
            GraphicsDevice graphics
        ) : Primitive<TVertexInstance>(sequence, sequenceGroup, staticVertexBuffer, staticIndexBuffers, bufferTypes, graphics),
            IPrimitive<TVertexInstance, TVertexStatic, TEffect>,
            IRuntimeSequence<PrimitiveSequenceGroupEnum>,
            IRuntimeSequenceGroup<PrimitiveSequenceGroupEnum>
        where TVertexInstance : unmanaged, IVertexType
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
        public TEffect Effect { get; } = effect;

        int IRuntimeSequence<PrimitiveSequenceGroupEnum>.Value => this.Sequence;
        SequenceGroup<PrimitiveSequenceGroupEnum> IRuntimeSequenceGroup<PrimitiveSequenceGroupEnum>.Value => SequenceGroup<PrimitiveSequenceGroupEnum>.GetByValue(this.SequenceGroup);

        public override void Draw(GameTime gameTime)
        {
            if (this.Flush() == false)
            {
                return;
            }

            this.Graphics.BlendState = BlendState.NonPremultiplied;

            for (int i = 0; i < this.VertexBufferBindings.Length; i++)
            {
                this.Graphics.SetVertexBuffers(this.VertexBufferBindings[i]);
                this.Graphics.Indices = this.StaticIndexBuffers[i];

                foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    this.Graphics.DrawInstancedPrimitives(this.BufferTypes[i], 0, 0, this.StaticPrimitiveCount[i], this.InstanceCount);
                }
            }

            this.Clear();
        }
    }
}

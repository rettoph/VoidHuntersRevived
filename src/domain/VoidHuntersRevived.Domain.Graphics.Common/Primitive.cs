using Guppy.Core.Common;
using Guppy.Core.Common.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Interfaces;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public abstract class Primitive
    {
        private static int FilterId;
        private static readonly FilterContextID FilterContextId = FilterContextID.GetNewContextID();

        private readonly CombinedFilterID _combinedFilterId;

        public readonly Type VertexType;
        public readonly int Sequence;
        public readonly VertexBuffer StaticVertexBuffer;
        public readonly IndexBuffer[] StaticIndexBuffers;
        public readonly PrimitiveTypeEnum[] BufferTypes;
        public readonly int[] StaticPrimitiveCount;
        public readonly GraphicsDevice Graphics;
        public readonly PrimitiveSequenceGroupEnum SequenceGroup;

        public int InstanceCount { get; protected set; }
        public VertexBuffer InstanceVertexBuffer { get; protected set; }
        public VertexBufferBinding[][] VertexBufferBindings { get; protected set; }

        internal Primitive(
            Type vertexType,
            int sequence,
            PrimitiveSequenceGroupEnum sequenceGroup,
            VertexBuffer staticVertexBuffer,
            IndexBuffer[] staticIndexBuffers,
            PrimitiveTypeEnum[] bufferTypes,
            GraphicsDevice graphics)
        {
            _combinedFilterId = new CombinedFilterID(FilterId++, FilterContextId);

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
        }

        public ref EntityFilterCollection GetFilter<TComponent>(EntitiesDB entitiesDb)
            where TComponent : unmanaged, IVertexType, IEntityComponent
        {
            return ref entitiesDb.GetFilters().GetOrCreatePersistentFilter<TComponent>(_combinedFilterId);
        }
    }

    public abstract class Primitive<TVertexInstance> : Primitive, IOnDrawPrimitive
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

        protected virtual bool Flush()
        {
            if (this.InstanceCount == 0)
            {
                return false;
            }

            this.InstanceVertexBuffer.SetData(_instanceVertices, 0, this.InstanceCount);
            return true;
        }

        protected virtual void Clear()
        {
            this.InstanceCount = 0;
        }

        public abstract void OnDraw(IDrawPrimitiveContext context);
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
            IRuntimeSequence<PrimitiveSequenceGroupEnum>,
            IRuntimeSequenceGroup<PrimitiveSequenceGroupEnum>
        where TVertexInstance : unmanaged, IVertexType
        where TVertexStatic : unmanaged, IVertexType
        where TEffect : Effect
    {
        public readonly TEffect Effect = effect;



        int IRuntimeSequence<PrimitiveSequenceGroupEnum>.Value => this.Sequence;
        SequenceGroup<PrimitiveSequenceGroupEnum> IRuntimeSequenceGroup<PrimitiveSequenceGroupEnum>.Value => SequenceGroup<PrimitiveSequenceGroupEnum>.GetByValue(this.SequenceGroup);

        public override void OnDraw(IDrawPrimitiveContext context)
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

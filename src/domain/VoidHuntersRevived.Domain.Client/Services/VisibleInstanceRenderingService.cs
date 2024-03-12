using Guppy.Game.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Client.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Client.Common.Services;
using VoidHuntersRevived.Domain.Client.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Domain.Client.Services
{
    internal sealed class VisibleInstanceRenderingService : BasicEngine, IVisibleInstanceRenderingService
    {
        private readonly IEntityService _entities;
        private readonly GraphicsDevice _graphics;
        private readonly VisibleEffect _effect;
        private readonly Camera2D _camera;
        private readonly BlendState _blendState;
        private readonly RasterizerState _rasterizerState;

        public VisibleInstanceRenderingService(IEntityService entities, GraphicsDevice graphics, VisibleEffect effect, Camera2D camera)
        {
            _entities = entities;
            _graphics = graphics;
            _effect = effect;
            _camera = camera;

            _blendState = BlendState.AlphaBlend;
            _rasterizerState = new RasterizerState()
            {
                MultiSampleAntiAlias = true,
                SlopeScaleDepthBias = 0.5f
            };
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            foreach (var ((staticEntities, typeIds, visibles, zIndices, count), group) in _entities.QueryEntities<StaticEntity, Id<IEntityType>, Visible, zIndex>())
            {
                for (int i = 0; i < count; i++)
                {
                    VisibleInstanceRenderingService.BuildStaticVertexBuffers(typeIds[i], visibles[i], zIndices[i], _graphics);
                }
            }
        }

        public void Begin()
        {
            _effect.View = _camera.View;
            _effect.Projection = _camera.Projection;
        }

        public void Draw(Id<IEntityType> entityTypeId, int count, ref EntityFilterCollection instances)
        {
            if (count == 0)
            {
                return;
            }

            VisibleInstanceRenderer renderer = _renderers[entityTypeId];
            VertexBufferBinding[] bindings = renderer.SetInstanceData(_entities, count, ref instances);

            _graphics.BlendState = _blendState;
            _graphics.RasterizerState = _rasterizerState;
            _graphics.SetVertexBuffers(bindings);
            _graphics.Indices = renderer.Indices;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, renderer.TriangleCount, renderer.InstanceCount);
            }
        }

        public void End()
        {
            _graphics.SetVertexBuffer(null);
        }

        private static readonly Dictionary<Id<IEntityType>, VisibleInstanceRenderer> _renderers = new Dictionary<Id<IEntityType>, VisibleInstanceRenderer>();
        private static readonly short[] _indexBuffer = new short[3];

        private static void Clear()
        {
            foreach (VisibleInstanceRenderer renderer in _renderers.Values)
            {
                renderer.Dispose();
            }

            _renderers.Clear();
        }

        private static void BuildStaticVertexBuffers(Id<IEntityType> entityType, Visible visible, zIndex zIndex, GraphicsDevice graphics)
        {
            if (_renderers.ContainsKey(entityType))
            {
                return;
            }

            List<VertexVisibleStatic> vertices = new List<VertexVisibleStatic>();
            int index = vertices.Count;
            int count = 0;
            List<short> indices = new List<short>();

            for (int shape_i = 0; shape_i < visible.Fill.count; shape_i++)
            {
                Shape shape = visible.Fill[shape_i];

                _indexBuffer[0] = (short)vertices.Count;
                vertices.Add(new VertexVisibleStatic(shape.Vertices[0], zIndex.Value));
                count++;

                _indexBuffer[1] = (short)vertices.Count;
                vertices.Add(new VertexVisibleStatic(shape.Vertices[1], zIndex.Value));
                count++;

                for (int vertex_i = 2; vertex_i < shape.Vertices.count; vertex_i++)
                {
                    _indexBuffer[2] = (short)vertices.Count;
                    vertices.Add(new VertexVisibleStatic(shape.Vertices[vertex_i], zIndex.Value));
                    count++;

                    indices.AddRange(_indexBuffer);
                    _indexBuffer[1] = _indexBuffer[2];
                }
            }

            VertexBuffer vertexBuffer = new VertexBuffer(graphics, typeof(VertexVisibleStatic), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());

            IndexBuffer indexBuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());

            _renderers.Add(entityType, new VisibleInstanceRenderer(graphics, vertexBuffer, indexBuffer));
        }
    }
}

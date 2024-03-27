using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Game.Client.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Common.Services;
using VoidHuntersRevived.Game.Client.Common.Utilities;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationFilter(SimulationType.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal sealed class VisibleVertexBufferManagerEngine : BasicEngine, IEngineEngine, IStepEngine<GameTime>, IDisposable,
        IVertexBufferManagerService<VertexInstanceVisible, Id<IEntityType>>
    {
        private readonly IEntityService _entities;
        private readonly Dictionary<Id<IEntityType>, VertexBufferManager<VertexInstanceVisible>> _managers;
        private readonly GraphicsDevice _graphics;
        private readonly GameWindow _window;
        private readonly Camera2D _camera;
        private IStepGroupEngine<IVertexBufferManagerService<VertexInstanceVisible, Id<IEntityType>>> _stepEngines;

        private RenderTarget2D _target_accum;
        private RenderTarget2D _target_top;
        private BlendState _bs_accum;
        private BlendState _bs_top;
        private BlendState _bs_final;

        private VisibleAccumEffect _effect_accum;
        private VisibleFinalEffect _effect_final;

        public string name { get; } = nameof(VisibleVertexBufferManagerEngine);

        public VisibleVertexBufferManagerEngine(
            IEntityService entities,
            GraphicsDevice graphics,
            GameWindow window,
            Camera2D camera,
            VisibleAccumEffect visibleAccumEffect,
            VisibleFinalEffect visibleFinalEffect)
        {
            _entities = entities;
            _graphics = graphics;
            _window = window;
            _camera = camera;
            _managers = new Dictionary<Id<IEntityType>, VertexBufferManager<VertexInstanceVisible>>();
            _stepEngines = null!;
            _effect_accum = visibleAccumEffect;
            _effect_final = visibleFinalEffect;

            _target_accum = new RenderTarget2D(graphics, graphics.Viewport.Width, graphics.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
            _bs_accum = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                ColorDestinationBlend = Blend.One
            };
            _bs_final = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.Zero,
                ColorDestinationBlend = Blend.Zero
            };

            _window.ClientSizeChanged += this.HandleClientSizeChanged;
        }

        public void Dispose()
        {
            _target_accum.Dispose();

            _window.ClientSizeChanged -= this.HandleClientSizeChanged;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            foreach (var ((staticEntities, typeIds, visibles, zIndices, count), group) in _entities.QueryEntities<StaticEntity, Id<IEntityType>, Visible, zIndex>())
            {
                for (int i = 0; i < count; i++)
                {
                    _managers.Add(typeIds[i], VisibleVertexBufferManagerEngine.BuildVertexBufferManager(typeIds[i], visibles[i], zIndices[i], _graphics));
                }
            }
        }

        public void Initialize(IEngine[] engines)
        {
            _stepEngines = engines.CreateSequencedStepEnginesGroup<IVertexBufferManagerService<VertexInstanceVisible, Id<IEntityType>>, DrawSequence>(DrawSequence.Draw);
        }

        public void Step(in GameTime param)
        {
            float scale = 1.5f / (-_camera.Zoom - 2) + 1;

            // Add vertices
            _stepEngines.Step(this);

            RenderTargetBinding[] targets_final = _graphics.GetRenderTargets();

            // Begin Pass Accum
            _graphics.SetRenderTarget(_target_accum);
            _graphics.Clear(Color.Transparent);
            _graphics.BlendState = _bs_accum = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                ColorDestinationBlend = Blend.One
            }; ;
            _graphics.DepthStencilState = DepthStencilState.None;
            _graphics.RasterizerState = RasterizerState.CullNone;
            _graphics.SamplerStates[0] = SamplerState.PointWrap;

            _effect_accum.TraceScale = scale;
            _effect_accum.TraceDiffusionScale = MathHelper.Lerp(scale, 1, 1f);
            _effect_accum.WorldViewProjection = _camera.World * _camera.View * _camera.Projection;

            foreach (VertexBufferManager<VertexInstanceVisible> manager in _managers.Values)
            {
                if (manager.InstanceCount > 0)
                {
                    manager.Flush();

                    _graphics.SetVertexBuffers(manager.VertexBufferBindings);
                    _graphics.Indices = manager.IndexBuffer;

                    foreach (EffectPass pass in _effect_accum.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        _graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, manager.StaticTriangleCount, manager.InstanceCount);
                    }
                }
            }

            // Begin Pass Final
            _graphics.SetRenderTargets(targets_final);
            _graphics.BlendState = BlendState.AlphaBlend;
            _graphics.DepthStencilState = DepthStencilState.Default;
            _graphics.RasterizerState = RasterizerState.CullNone;
            _graphics.SamplerStates[0] = SamplerState.PointWrap;

            _effect_final.TraceScale = scale;
            _effect_final.TraceDiffusionScale = MathHelper.Lerp(scale, 1, 0.75f);
            _effect_final.WorldViewProjection = _camera.World * _camera.View * _camera.Projection;
            _effect_final.AccumTexture = _target_accum;

            foreach (VertexBufferManager<VertexInstanceVisible> manager in _managers.Values)
            {
                if (manager.InstanceCount > 0)
                {
                    _graphics.SetVertexBuffers(manager.VertexBufferBindings);
                    _graphics.Indices = manager.IndexBuffer;

                    foreach (EffectPass pass in _effect_final.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        _graphics.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, manager.StaticTriangleCount, manager.InstanceCount);
                    }

                    manager.Clear();
                }
            }

            // var mouse = Mouse.GetState().Position;
            // Vector4[] data = new Vector4[_target_accum.Width * _target_accum.Height];
            // _target_accum.GetData(data);
            // int index = mouse.X + (mouse.Y * _target_accum.Width);
            // 
            // if (index >= 0 && index < data.Length)
            // {
            //     Console.WriteLine(data[index]);
            // }
        }

        public VertexBufferManager<VertexInstanceVisible> GetById(Id<IEntityType> id)
        {
            return _managers[id];
        }

        private void HandleClientSizeChanged(object? sender, EventArgs e)
        {
            _target_accum.Dispose();
            _target_accum = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
        }

        private static readonly short[] _indexBuffer = new short[10];
        private static VertexBufferManager<VertexInstanceVisible> BuildVertexBufferManager(Id<IEntityType> entityType, Visible visible, zIndex zIndex, GraphicsDevice graphics)
        {
            List<VertexStaticVisible> vertices = new List<VertexStaticVisible>();
            int index = vertices.Count;
            int count = 0;
            List<short> indices = new List<short>();

            for (int shape_i = 0; shape_i < visible.Fill.count; shape_i++)
            {
                Shape shape = visible.Fill[shape_i];

                _indexBuffer[0] = (short)vertices.Count;
                vertices.Add(new VertexStaticVisible(shape.Vertices[0], zIndex.Value));
                count++;

                _indexBuffer[1] = (short)vertices.Count;
                vertices.Add(new VertexStaticVisible(shape.Vertices[1], zIndex.Value));
                count++;

                for (int vertex_i = 2; vertex_i < shape.Vertices.count; vertex_i++)
                {
                    _indexBuffer[2] = (short)vertices.Count;
                    vertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i], zIndex.Value));
                    count++;

                    indices.AddRange(_indexBuffer[..3]);
                    _indexBuffer[1] = _indexBuffer[2];
                }
            }

            for (int shape_i = 0; shape_i < visible.TraceVertices.count; shape_i++)
            {
                Shape shape = visible.TraceVertices[shape_i];
                int offset1 = 0;
                int offset2 = 5;
                int placeholder = 0;

                _indexBuffer[offset1 + 0] = (short)vertices.Count;
                vertices.Add(new VertexStaticVisible(shape.Vertices[offset1 + 0], zIndex.Value, true, true));

                _indexBuffer[offset1 + 1] = (short)vertices.Count;
                vertices.Add(new VertexStaticVisible(shape.Vertices[offset1 + 1], zIndex.Value, true, true));

                _indexBuffer[offset1 + 2] = (short)vertices.Count;
                vertices.Add(new VertexStaticVisible(shape.Vertices[offset1 + 2], zIndex.Value, true, true));

                _indexBuffer[offset1 + 3] = (short)vertices.Count;
                vertices.Add(new VertexStaticVisible(shape.Vertices[offset1 + 3], zIndex.Value, true, true));

                _indexBuffer[offset1 + 4] = (short)vertices.Count;
                vertices.Add(new VertexStaticVisible(shape.Vertices[offset1 + 4], zIndex.Value, true, false));

                count += 5;

                for (int vertex_i = 5; vertex_i < shape.Vertices.count; vertex_i += 5)
                {
                    _indexBuffer[offset2 + 0] = (short)vertices.Count;
                    vertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i + 0], zIndex.Value, true, true));

                    _indexBuffer[offset2 + 1] = (short)vertices.Count;
                    vertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i + 1], zIndex.Value, true, true));

                    _indexBuffer[offset2 + 2] = (short)vertices.Count;
                    vertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i + 2], zIndex.Value, true, true));

                    _indexBuffer[offset2 + 3] = (short)vertices.Count;
                    vertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i + 3], zIndex.Value, true, true));

                    _indexBuffer[offset2 + 4] = (short)vertices.Count;
                    vertices.Add(new VertexStaticVisible(shape.Vertices[vertex_i + 4], zIndex.Value, true, false));

                    count += 5;

                    indices.Add(_indexBuffer[offset1 + 4]);
                    indices.Add(_indexBuffer[offset1 + 1]);
                    indices.Add(_indexBuffer[offset1 + 2]);

                    indices.Add(_indexBuffer[offset1 + 4]);
                    indices.Add(_indexBuffer[offset1 + 2]);
                    indices.Add(_indexBuffer[offset1 + 3]);

                    indices.Add(_indexBuffer[offset1 + 4]);
                    indices.Add(_indexBuffer[offset1 + 3]);
                    indices.Add(_indexBuffer[offset2 + 1]);

                    indices.Add(_indexBuffer[offset2 + 1]);
                    indices.Add(_indexBuffer[offset2 + 4]);
                    indices.Add(_indexBuffer[offset1 + 4]);

                    indices.Add(_indexBuffer[offset1 + 0]);
                    indices.Add(_indexBuffer[offset1 + 4]);
                    indices.Add(_indexBuffer[offset2 + 4]);

                    indices.Add(_indexBuffer[offset2 + 4]);
                    indices.Add(_indexBuffer[offset2 + 0]);
                    indices.Add(_indexBuffer[offset1 + 0]);

                    placeholder = offset1;
                    offset1 = offset2;
                    offset2 = placeholder;
                }
            }

            VertexBuffer vertexBuffer = new VertexBuffer(graphics, typeof(VertexStaticVisible), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());

            IndexBuffer indexBuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());

            return new VertexBufferManager<VertexInstanceVisible>(graphics, vertexBuffer, indexBuffer);
        }
    }
}

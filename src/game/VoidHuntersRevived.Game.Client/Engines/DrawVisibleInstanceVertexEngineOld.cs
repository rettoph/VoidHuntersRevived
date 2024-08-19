using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Game.Client.Common.Engines;
using VoidHuntersRevived.Game.Client.Common.Services;
using VoidHuntersRevived.Game.Client.Common.Utilities;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal sealed class DrawVisibleInstanceVertexEngineOld : BaseDrawInstanceVertexEngine<IVisibleInstanceVertexService>
    {
        private readonly GraphicsDevice _graphics;
        private readonly GameWindow _window;
        private readonly Camera2D _camera;

        private RenderTarget2D _target_accum;
        private RenderTarget2D _target_top;
        private BlendState _bs_accum;
        private BlendState _bs_final;

        private VisibleAccumEffect _effect_accum;
        private VisibleFinalEffect _effect_final;

        public DrawVisibleInstanceVertexEngineOld(
            IVisibleInstanceVertexService instancePrimitiveVertexService,
            GraphicsDevice graphics,
            GameWindow window,
            Camera2D camera,
            VisibleAccumEffect visibleAccumEffect,
            VisibleFinalEffect visibleFinalEffect) : base(instancePrimitiveVertexService)
        {
            _graphics = graphics;
            _window = window;
            _camera = camera;
            _effect_accum = visibleAccumEffect;
            _effect_final = visibleFinalEffect;

            this.BuildRenderTargets(out _target_accum, out _target_top);

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
            _target_top.Dispose();

            _window.ClientSizeChanged -= this.HandleClientSizeChanged;
        }


        private void BuildRenderTargets(out RenderTarget2D target_accum, out RenderTarget2D target_top)
        {
            target_accum = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height, true, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            target_top = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height, true, SurfaceFormat.Vector4, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
        }

        protected override void Draw(GameTime gameTime)
        {
            RenderTargetBinding[] targets_final = _graphics.GetRenderTargets();

            // Begin Pass Accum
            _graphics.SetRenderTarget(_target_accum);
            _graphics.Clear(Color.Transparent);
            _graphics.BlendState = _bs_accum;
            _graphics.DepthStencilState = DepthStencilState.None;
            _graphics.RasterizerState = RasterizerState.CullNone;
            _graphics.SamplerStates[0] = SamplerState.AnisotropicWrap;

            _effect_accum.WorldViewProjection = _camera.World * _camera.View * _camera.Projection;

            foreach (InstanceVertexProvider<VertexInstanceVisible> visibleInstanceVertexProvider in this.instancedPrimitiveVertexService.GetAllInstanceVertexProviders())
            {
                if (visibleInstanceVertexProvider.InstanceCount > 0)
                {
                    visibleInstanceVertexProvider.Flush();

                    for (int i = 0; i < visibleInstanceVertexProvider.BufferCount; i++)
                    {
                        _graphics.SetVertexBuffers(visibleInstanceVertexProvider.VertexBufferBindings[i]);
                        _graphics.Indices = visibleInstanceVertexProvider.IndexBuffers[i];

                        foreach (EffectPass pass in _effect_accum.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            _graphics.DrawInstancedPrimitives(visibleInstanceVertexProvider.PrimitiveTypes[i], 0, 0, visibleInstanceVertexProvider.StaticPrimitiveCount[i], visibleInstanceVertexProvider.InstanceCount);
                        }
                    }
                }
            }

            // Begin Pass Final
            _graphics.SetRenderTargets(targets_final);
            _graphics.BlendState = _bs_final;
            _graphics.DepthStencilState = DepthStencilState.Default;
            _graphics.RasterizerState = RasterizerState.CullNone;
            _graphics.SamplerStates[0] = SamplerState.PointClamp;

            _effect_final.HideTop = false;
            _effect_final.HideAccum = false;
            _effect_final.WorldViewProjection = _camera.World * _camera.View * _camera.Projection;
            _effect_final.AccumTexture = _target_accum;

            foreach (InstanceVertexProvider<VertexInstanceVisible> manager in this.instancedPrimitiveVertexService.GetAllInstanceVertexProviders())
            {
                if (manager.InstanceCount > 0)
                {
                    for (int i = 0; i < manager.BufferCount; i++)
                    {
                        _graphics.SetVertexBuffers(manager.VertexBufferBindings[i]);
                        _graphics.Indices = manager.IndexBuffers[i];

                        foreach (EffectPass pass in _effect_final.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            _graphics.DrawInstancedPrimitives(manager.PrimitiveTypes[i], 0, 0, manager.StaticPrimitiveCount[i], manager.InstanceCount);
                        }
                    }

                    manager.Clear();
                }
            }
        }

        private void HandleClientSizeChanged(object? sender, EventArgs e)
        {
            _target_accum.Dispose();
            _target_top.Dispose();

            this.BuildRenderTargets(out _target_accum, out _target_top);
        }
    }
}

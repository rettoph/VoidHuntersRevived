using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Engines;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    [Sequence<UpdateSequence>(UpdateSequence.PostUpdate)]
    public class DrawVertexInstanceVisibleEngine : BaseDrawVertexTypeEngine<VertexInstanceVisible>, IStepEngine<Step>
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly Camera2D _camera;
        private readonly GraphicsDevice _graphics;

        protected override VisibleEffect effect { get; }

        public DrawVertexInstanceVisibleEngine(
            IEntityQueryService entityQueryService,
            IVertexTypeService vertexTypeService,
            Camera2D camera,
            GraphicsDevice graphics,
            GameWindow window,
            SpriteBatch spriteBatch,
            VisibleEffect effect) : base(vertexTypeService)
        {
            _entityQueryService = entityQueryService;
            _camera = camera;
            _graphics = graphics;

            this.effect = effect;
        }

        public override void Step(in GameTime param)
        {
            // RenderTargetBinding[] original_targets = _graphics.GetRenderTargets();

            // _graphics.SetRenderTarget(_target);

            //_graphics.Clear(Color.Transparent);
            _graphics.BlendState = BlendState.NonPremultiplied;
            // _graphics.DepthStencilState = DepthStencilState.None;
            // _graphics.RasterizerState = RasterizerState.CullNone;
            // _graphics.SamplerStates[0] = SamplerState.AnisotropicWrap;

            this.effect.WorldViewProjection = _camera.World * _camera.View * _camera.Projection;

            base.Step(param);

            // _graphics.SetRenderTargets(original_targets);

            // _spriteBatch.Begin();
            // _spriteBatch.Draw(_target, Vector2.Zero, Color.White);
            // _spriteBatch.End();
        }

        /// <summary>
        /// Copy Svelto entity data to vertex
        /// </summary>
        /// <param name="param"></param>
        public void Step(in Step param)
        {
            foreach (var ((vertices, colorSchemes, nodes, statuses, count), _) in _entityQueryService.QueryEntities<VertexInstanceVisible, ColorScheme, Node>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref VertexInstanceVisible vertex = ref vertices[i];
                    ref ColorScheme colorScheme = ref colorSchemes[i];
                    ref Node node = ref nodes[i];

                    vertex.LocalTransformation = node.XnaTransformation;
                    vertex.PrimaryColor = colorScheme.Primary.Value.PackedValue;
                    vertex.SecondaryColor = colorScheme.Secondary.Value.PackedValue;
                }
            }
        }
    }
}

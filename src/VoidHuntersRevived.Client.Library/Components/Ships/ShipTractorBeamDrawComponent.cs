using Guppy.EntityComponent.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Components.Ships
{
    internal sealed class ShipTractorBeamDrawComponent : FrameableDrawComponent<Ship>
    {
        private SpriteFont _font;
        private ShipPartRenderService _renderer;

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _renderer);

            _font = provider.GetContent<SpriteFont>(Guppy.Constants.Content.DebugFont);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            TargetComponent target = this.Entity.Components.Get<TargetComponent>();
            TractorBeamComponent tractorBeam = this.Entity.Components.Get<TractorBeamComponent>();

            ConnectionNode targetNode = tractorBeam.GetMostViableConnectionNodeTarget();
            ShipPart targetPart = tractorBeam.GetMostViableShipPartTarget();
            
            if (targetNode is not null)
            {
                this.primitiveBatch.TraceCircle(
                    Color.White,
                    targetNode.Owner.CalculateWorldPoint(targetNode.LocalPosition),
                    0.25f,
                    25);
            }

            if(targetPart is not null)
            {
                this.primitiveBatch.TraceCircle(
                    Color.White,
                    targetPart.CalculateWorldPoint(
                        targetPart.Context.Centeroid,
                        targetPart.Chain.WorldTransformation),
                    1f,
                    25);
            }
        }
    }
}

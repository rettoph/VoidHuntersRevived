using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Network.Peers;
using Guppy.Network.Structs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Ships
{
    internal sealed class ShipTractorBeamDrawComponent : FrameableDrawComponent<Ship>
    {
        private SpriteFont _font;
        private Peer _peer;
        DiagnosticIntervalData _d;
        UInt32[] _r;
        UInt32 _rPos;

        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            _font = provider.GetContent<SpriteFont>(Guppy.Constants.Content.DebugFont);

            provider.Service(out _peer);

            _peer.OnDiagnosticInterval += (_, d) =>
            {
                _d = d;
                _r[_rPos++ % 10] = _d.Recieved;
            };

            _r = new UInt32[10];
            _rPos = 0;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ShipTractorBeamComponent tractorBeam = this.Entity.Components.Get<ShipTractorBeamComponent>();
            ConnectionNode targetNode = tractorBeam.GetConnectionNodeTarget();

            var sb = new StringBuilder();
            sb.AppendLine($"{tractorBeam.Position}\n");
            sb.AppendLine($"Messages Recieved (10s Avg.): {_r.Average(r => r)}\n");

            if (targetNode != default)
            {
                this.primitiveBatch.TraceCircle(
                    Color.White,
                    targetNode.Owner.CalculateWorldPoint(targetNode.LocalPosition),
                    0.25f,
                    25);


                
                sb.AppendLine($"      Target.Id: {targetNode.Owner.Id}");
                sb.AppendLine($"Target.Chain.Id: {targetNode.Owner.Chain.Id}");
                sb.AppendLine($"        Root.Id: {targetNode.Owner.Root.Id}");
                sb.AppendLine($"  Root.Chain.Id: {targetNode.Owner.Root.Chain.Id}\n");

                sb.AppendLine($"Aether Data");
                sb.AppendLine(targetNode.Owner.ToAetherString());
            }

            this.spriteBatch.DrawString(
                spriteFont: _font,
                text: sb.ToString(),
                position: this.camera.Unproject(Vector2.One * 25),
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1 / this.camera.Zoom,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}

using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Special
{
    internal sealed class SpellCasterPartGraphicsDriver : Driver<SpellCasterPart>
    {
        #region Static Fields
        private static readonly Int32 MaxTimerSegments = 25;
        #endregion

        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private ILog _log;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(SpellCasterPart driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _log);

            this.driven.OnPostDraw += this.PostDraw;
        }

        protected override void Release(SpellCasterPart driven)
        {
            base.Release(driven);

            _primitiveBatch = null;

            this.driven.OnPostDraw -= this.PostDraw;
        }
        #endregion

        #region Frame Methods
        private void PostDraw(GameTime gameTime)
        {
            var cooldownPercent = ((Single)gameTime.TotalGameTime.TotalSeconds - this.driven.LastCastTimestamp) / this.driven.Context.SpellCooldown;

            var radius = new Vector2(0.4f, 0);
            var angle = MathHelper.TwoPi * cooldownPercent;
            var segmentCount = (Int32)(SpellCasterPartGraphicsDriver.MaxTimerSegments * cooldownPercent);
            var segment = angle / segmentCount;

            var oldSegment = this.driven.WorldCenter + radius;
            var newSegment = oldSegment;
            for (var i=1; i<segmentCount; i++)
            {
                newSegment = this.driven.WorldCenter + radius.RotateTo(segment * i);
                _primitiveBatch.DrawTriangle(Color.Red, this.driven.WorldCenter, oldSegment, newSegment);
                oldSegment = newSegment;
            }
        }
        #endregion
    }
}

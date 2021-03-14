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
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.SpellParts
{
    internal sealed class SpellPartGraphicsDriver : Driver<SpellPart>
    {
        #region Static Fields
        private static readonly Int32 MaxTimerSegments = 25;
        private static readonly Single TimerSegmentRadians = MathHelper.TwoPi / SpellPartGraphicsDriver.MaxTimerSegments;
        #endregion

        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private ILog _log;
        private Vector2[] _segments;
        private Vector2 _radius;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(SpellPart driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _log);

            _radius = new Vector2(this.driven.Context.IndicatorRadius, 0);
            _segments = new Vector2[SpellPartGraphicsDriver.MaxTimerSegments];

            for (Int32 i = 0; i< SpellPartGraphicsDriver.MaxTimerSegments; i++)
                _segments[i] = _radius.RotateTo(SpellPartGraphicsDriver.TimerSegmentRadians * i);

            this.driven.OnPostDraw += this.PostDraw;
        }

        protected override void Release(SpellPart driven)
        {
            base.Release(driven);

            _primitiveBatch = null;

            this.driven.OnPostDraw -= this.PostDraw;
        }
        #endregion

        #region Frame Methods
        private void PostDraw(GameTime gameTime)
        {
            if (this.driven.LastCastTimestamp == default || this.driven.Chain.Ship == default)
                return;

            var cooldownPercent = 1 - Math.Min(1, ((Single)gameTime.TotalGameTime.TotalSeconds - this.driven.LastCastTimestamp) / this.driven.Context.SpellCooldown);

            if (cooldownPercent == 0)
                return;

            var target = MathHelper.TwoPi * cooldownPercent;
            var segment = (Int32)Math.Ceiling(SpellPartGraphicsDriver.MaxTimerSegments * cooldownPercent);
            var offset = target % SpellPartGraphicsDriver.TimerSegmentRadians;
            var oldSegment = this.driven.WorldCenter + _segments[0];
            var newSegment = Vector2.Zero;
            var color = new Color(Color.Black, 0.5f);

            for (var i = 1; i < segment; i++)
            {
                newSegment = this.driven.WorldCenter + _segments[i];

                _primitiveBatch.DrawTriangle(
                    color,
                    this.driven.WorldCenter,
                    oldSegment,
                    newSegment);

                oldSegment = newSegment;
            }

            _primitiveBatch.DrawTriangle(
                    color,
                    this.driven.WorldCenter,
                    oldSegment,
                    this.driven.WorldCenter + _radius.RotateTo(target));
        }
        #endregion
    }
}

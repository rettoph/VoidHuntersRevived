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

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Special
{
    internal sealed class SpellCasterPartGraphicsDriver : Driver<SpellPart>
    {
        #region Static Fields
        private static readonly Int32 MaxTimerSegments = 25;
        private static readonly Single TimerSegmentRadians = MathHelper.TwoPi / SpellCasterPartGraphicsDriver.MaxTimerSegments;
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

            _radius = new Vector2(0.4f, 0);
            _segments = new Vector2[SpellCasterPartGraphicsDriver.MaxTimerSegments];

            for (Int32 i = 0; i< SpellCasterPartGraphicsDriver.MaxTimerSegments; i++)
                _segments[i] = _radius.RotateTo(SpellCasterPartGraphicsDriver.TimerSegmentRadians * i);

            this.driven.OnPostDraw += this.PostDraw;
            this.driven.OnChainChanged += this.HandleChainChanged;

            this.CleanShip(default, this.driven.Chain?.Ship);
        }

        protected override void Release(SpellPart driven)
        {
            base.Release(driven);

            _primitiveBatch = null;

            this.driven.OnPostDraw -= this.PostDraw;
            this.driven.OnChainChanged -= this.HandleChainChanged;
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
            var segment = (Int32)Math.Ceiling(SpellCasterPartGraphicsDriver.MaxTimerSegments * cooldownPercent);
            var offset = target % SpellCasterPartGraphicsDriver.TimerSegmentRadians;
            var oldSegment = this.driven.WorldCenter + _segments[0];
            var newSegment = Vector2.Zero;
            var color = Color.Lerp(this.driven.Chain.Ship.Color, Color.Black, 0.5f);

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

        #region Helper Methods
        private void CleanShip(Ship old, Ship value)
        {
            if(value == default && old != default)
            {
                this.driven.OnPostDraw -= this.PostDraw;
            }
            else if(old == default && value != default)
            {
                this.driven.OnPostDraw += this.PostDraw;
            }
        }
        #endregion

        #region Event Handlers
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if(old != default)
            {
                old.OnShipChanged -= this.HandleShipChanged;
            }

            if(value != default)
            {
                old.OnShipChanged += this.HandleShipChanged;
            }

            this.CleanShip(old?.Ship, value?.Ship);
        }

        private void HandleShipChanged(Chain sender, Ship old, Ship value)
            => this.CleanShip(old, value);
        #endregion
    }
}
